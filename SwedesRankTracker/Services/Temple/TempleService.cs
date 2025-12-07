using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using SwedesRankTracker.Models;

namespace SwedesRankTracker.Services.Temple
{
    public class TempleService : ITempleService
    {
        private readonly HttpClient _httpClient;

        // Throttling / retry configuration
        private static readonly SemaphoreSlim _throttle = new SemaphoreSlim(1, 1);
        private static DateTime _lastRequestTime = DateTime.MinValue;
        private static readonly TimeSpan _minIntervalBetweenRequests = TimeSpan.FromMilliseconds(500); // adjust as needed
        private static readonly TimeSpan _initialRetryDelay = TimeSpan.FromSeconds(1);
        private const int _maxRetryAttempts = 5;
        private static readonly Random _jitter = new Random();

        public TempleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetClanMemberUsernamesAsync()
        {
            try
            {
                // Use throttled helper to call endpoint
                var clan = await GetFromApiWithThrottleAsync<List<string>>("groupmembers.php?id=449");
                return clan ?? new List<string>();
            }
            catch (TooManyRequestsException)
            {
                // Bubble up explicit too-many-requests to caller (could be handled higher, logged, etc.)
                throw;
            }
            catch (HttpRequestException)
            {
                // decide how you want to handle errors (log, retry, bubble up, etc.)
                return new List<string>();
            }
        }

        public async Task<Member> GetMemberDataAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("username is required", nameof(username));

            try
            {
                var playerDto = await GetFromApiWithThrottleAsync<TemplePlayerApiResponse>($"player_stats.php?player={Uri.EscapeDataString(username)}&bosses=1");
                var playerPetsDto = await GetFromApiWithThrottleAsync<TemplePetApiResponse>($"collection-log/player_collection_log.php?player={Uri.EscapeDataString(username)}&categories=all_pets");

                var info = playerDto?.Data?.Info;
                var ehb = playerDto?.Data?.Ehb;
                var ehp = playerDto?.Data?.Ehp;
                var collections = playerDto?.Data?.Collections;
                var pets = playerPetsDto?.Data?.Items?.ListOfPets?.Count;
                var totalLevel = playerDto?.Data?.TotalLevel;

                // Map to domain Member. Use safe defaults if API didn't include values.
                return new Member
                {
                    UserName = info?.Username ?? username,
                    Ehb = ehb.HasValue ? (int)Math.Round(ehb.Value) : 0,
                    Ehp = ehp.HasValue ? (int)Math.Round(ehp.Value) : 0,
                    TotalLevel = totalLevel.HasValue ? (int)Math.Round(totalLevel.Value) : 0,
                    Pets = pets.HasValue ? pets.Value : 0,
                    Collections = collections.HasValue ? (int)Math.Round(collections.Value) : 0,
                    LastUpdated = DateTime.UtcNow,
                };
            }
            catch (TooManyRequestsException)
            {
                // propagate to caller to allow higher-level handling (e.g., UI message, retry scheduling)
                throw;
            }
            catch (HttpRequestException)
            {
                // Decide whether to return defaults or rethrow; keeping original behavior of rethrowing.
                throw;
            }
        }

        // Helper method that performs GET with throttling and retry logic
        private async Task<T?> GetFromApiWithThrottleAsync<T>(string uri)
        {
            await _throttle.WaitAsync();
            try
            {
                // Enforce minimum interval between requests
                var timeSinceLast = DateTime.UtcNow - _lastRequestTime;
                if (timeSinceLast < _minIntervalBetweenRequests)
                {
                    var wait = _minIntervalBetweenRequests - timeSinceLast;
                    await Task.Delay(wait);
                }

                var attempt = 0;
                var delay = _initialRetryDelay;

                while (true)
                {
                    attempt++;
                    using var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                    _lastRequestTime = DateTime.UtcNow;

                    if (response.IsSuccessStatusCode)
                    {
                        // Read and deserialize JSON body
                        return await response.Content.ReadFromJsonAsync<T>();
                    }

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        // Too Many Requests: respect Retry-After if present, otherwise exponential backoff
                        if (attempt >= _maxRetryAttempts)
                        {
                            Console.WriteLine($"Received 429 Too Many Requests from API for '{uri}' after {attempt} attempts.");
                            throw new TooManyRequestsException();
                        }

                        var retryAfter = response.Headers.RetryAfter?.Delta;
                        if (retryAfter.HasValue)
                        {
                            await Task.Delay(retryAfter.Value);
                        }
                        else
                        {
                            // backoff + jitter
                            await Task.Delay(delay + TimeSpan.FromMilliseconds(_jitter.Next(0, 500)));
                            delay = TimeSpan.FromTicks(delay.Ticks * 2);
                        }

                        continue;
                    }

                    // Transient server errors (5xx) -> retry
                    if ((int)response.StatusCode >= 500 && (int)response.StatusCode < 600 && attempt < _maxRetryAttempts)
                    {
                        await Task.Delay(delay + TimeSpan.FromMilliseconds(_jitter.Next(0, 500)));
                        delay = TimeSpan.FromTicks(delay.Ticks * 2);
                        continue;
                    }

                    // For other failures, throw to let caller handle
                    response.EnsureSuccessStatusCode();
                }
            }
            finally
            {
                _throttle.Release();
            }
        }
    }

    // Custom exception for explicit handling of exhausted 429 retries
    public sealed class TooManyRequestsException : HttpRequestException
    {
        public TooManyRequestsException()
        {
        }

        public TooManyRequestsException(string? message)
            : base(message)
        {
        }

        public TooManyRequestsException(string? message, Exception? inner)
            : base(message, inner)
        {
        }
    }
}