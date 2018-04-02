using System.Threading.Tasks;

namespace Orleans.ConverterContracts
{
    public interface IConverter : IGrainWithGuidKey
    {
        Task<double> ConvertToMile(double value);

        Task<double> ConvertToKm(double value);
    }
}