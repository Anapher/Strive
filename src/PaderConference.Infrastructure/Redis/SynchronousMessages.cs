using System;
using System.Threading.Tasks;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis
{
    public static class SynchronousMessages
    {
        public static async Task<SuccessOrError<TResult>> InvokeAsync<TResult, T>(this IRedisDatabase database,
            string channel, T message)
        {
            var messageId = Guid.NewGuid().ToString("N");
            var responseChannel = new RedisChannel($"RESPONSE::{messageId}", RedisChannel.PatternMode.Literal);

            var completionSource = new TaskCompletionSource<SuccessOrError<TResult>>();

            Task OnProcessResponse(SuccessOrError<TResult> arg)
            {
                completionSource.SetResult(arg);
                return Task.CompletedTask;
            }

            await database.SubscribeAsync<SuccessOrError<TResult>>(responseChannel, OnProcessResponse);

            try
            {
                await database.PublishAsync(channel, new CallbackMessage<T>(message, responseChannel.ToString()));
                return await completionSource.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
            }
            finally
            {
                await database.UnsubscribeAsync<SuccessOrError<TResult>>(responseChannel, OnProcessResponse);
            }
        }

        public static Task InvokeAsync<T>(this IRedisDatabase database, string channel, T message)
        {
            return InvokeAsync<object, T>(database, channel, message);
        }
    }

    public class CallbackMessage<T>
    {
        public CallbackMessage(T payload, string callbackChannel)
        {
            Payload = payload;
            CallbackChannel = callbackChannel;
        }

        public T Payload { get; }

        public string CallbackChannel { get; }
    }
}