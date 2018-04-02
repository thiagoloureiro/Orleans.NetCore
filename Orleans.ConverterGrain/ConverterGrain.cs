using Orleans.ConverterContracts;
using System.Threading.Tasks;

namespace Orleans.ConverterGrain
{
    public class ConverterGrain : Grain, IConverter
    {
        public Task<double> ConvertToKm(double value)
        {
            var result = value * 1.6;
            return Task.FromResult(result);
        }

        public Task<double> ConvertToMile(double value)
        {
            var result = value / 1.6;
            return Task.FromResult(result);
        }
    }
}