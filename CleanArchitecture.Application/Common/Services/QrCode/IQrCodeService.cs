namespace CleanArchitecture.Application.Common.Services.QrCode
{
    public interface IQrCodeService
    {
        byte[] Generate(string content);
    }
}
