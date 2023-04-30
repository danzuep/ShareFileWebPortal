using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Helpers
{
    public interface ICustomMapper<in TInput, out TOutput>
    {
        TOutput Map(TInput input);
    }

    //https://www.siepman.nl/blog/imapperin-tinput-out-toutput-interface-with-handy-extensions
    public static class MapperExtensions
    {
        public static IEnumerable<TOut> Map<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, IEnumerable<TIn> values)
        {
            return values.Select(mapper.Map);
        }

        public static IEnumerable<TOut> Map<TIn, TOut>(this IEnumerable<TIn> values, ICustomMapper<TIn, TOut> mapper)
        {
            return values.Select(mapper.Map);
        }
        public static TOut Map<TIn, TOut>(this TIn input, ICustomMapper<TIn, TOut> mapper)
        {
            return mapper.Map(input);
        }

        public static List<TOut> MapToList<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, IEnumerable<TIn> values)
        {
            return values.Map(mapper).ToList();
        }

        public static List<TOut> MapToList<TIn, TOut>(this IEnumerable<TIn> values, ICustomMapper<TIn, TOut> mapper)
        {
            return values.Map(mapper).ToList();
        }

        public static IReadOnlyList<TOut> MapToReadOnlyList<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, IEnumerable<TIn> values)
        {
            return values.MapToList(mapper);
        }

        public static IReadOnlyList<TOut> MapToReadOnlyList<TIn, TOut>(this IEnumerable<TIn> values, ICustomMapper<TIn, TOut> mapper)
        {
            return values.MapToList(mapper);
        }

        public static async Task<IEnumerable<TOut>> Map<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, Task<IEnumerable<TIn>> valuesTask)
        {
            var values = await valuesTask;
            return mapper.Map(values);
        }

        public static async Task<IEnumerable<TOut>> Map<TIn, TOut>(this Task<IEnumerable<TIn>> valuesTask, ICustomMapper<TIn, TOut> mapper)
        {
            return await mapper.Map(valuesTask);
        }

        public static async Task<TOut> Map<TIn, TOut>(this Task<TIn> inputTask, ICustomMapper<TIn, TOut> mapper)
        {
            var input = await inputTask;
            return mapper.Map(input);
        }

        public static async Task<TOut> Map<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, Task<TIn> inputTask)
        {
            return await inputTask.Map(mapper);
        }

        public static async Task<List<TOut>> MapToList<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, Task<IEnumerable<TIn>> valuesTask)
        {
            return (await valuesTask.Map(mapper)).ToList();
        }

        public static async Task<List<TOut>> MapToList<TIn, TOut>(this Task<IEnumerable<TIn>> valuesTask, ICustomMapper<TIn, TOut> mapper)
        {
            return await MapToList(mapper, valuesTask);
        }

        public static async Task<List<TOut>> MapToReadOnlyList<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, Task<IEnumerable<TIn>> valuesTask)
        {
            return await valuesTask.MapToList(mapper);
        }

        public static async Task<List<TOut>> MapToReadOnlyList<TIn, TOut>(this Task<IEnumerable<TIn>> valuesTask, ICustomMapper<TIn, TOut> mapper)
        {
            return await valuesTask.MapToList(mapper);
        }

        public static async Task<List<TOut>> MapToList<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, Task<List<TIn>> valuesTask)
        {
            return (await valuesTask).MapToList(mapper);
        }

        public static async Task<List<TOut>> MapToList<TIn, TOut>(this Task<List<TIn>> valuesTask, ICustomMapper<TIn, TOut> mapper)
        {
            return (await valuesTask).MapToList(mapper);
        }

        public static async Task<IReadOnlyList<TOut>> MapToReadOnlyList<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, Task<List<TIn>> valuesTask)
        {
            return (await valuesTask).MapToList(mapper);
        }

        public static async Task<IReadOnlyList<TOut>> MapToReadOnlyList<TIn, TOut>(this Task<List<TIn>> valuesTask, ICustomMapper<TIn, TOut> mapper)
        {
            return (await valuesTask).MapToList(mapper);
        }

        public static async Task<IReadOnlyList<TOut>> MapToReadOnlyList<TIn, TOut>(this ICustomMapper<TIn, TOut> mapper, Task<IReadOnlyList<TIn>> valuesTask)
        {
            return (await valuesTask).MapToList(mapper);
        }

        public static async Task<IReadOnlyList<TOut>> MapToReadOnlyList<TIn, TOut>(this Task<IReadOnlyList<TIn>> valuesTask, ICustomMapper<TIn, TOut> mapper)
        {
            return (await valuesTask).MapToList(mapper);
        }

        public static IEnumerable<TOut> MapMany<TIn, TOut>(this ICustomMapper<TIn, IEnumerable<TOut>> mapper, IEnumerable<TIn> values)
        {
            return values.SelectMany(mapper.Map);
        }

        public static IEnumerable<TOut> MapMany<TIn, TOut>(this IEnumerable<TIn> values, ICustomMapper<TIn, IEnumerable<TOut>> mapper)
        {
            return values.SelectMany(mapper.Map);
        }

        public static IEnumerable<TOut> MapManyToList<TIn, TOut>(this ICustomMapper<TIn, IEnumerable<TOut>> mapper, IEnumerable<TIn> values)
        {
            return values.SelectMany(mapper.Map).ToList();
        }

        public static IEnumerable<TOut> MapManyToList<TIn, TOut>(this IEnumerable<TIn> values, ICustomMapper<TIn, IEnumerable<TOut>> mapper)
        {
            return values.SelectMany(mapper.Map).ToList();
        }
    }
}
