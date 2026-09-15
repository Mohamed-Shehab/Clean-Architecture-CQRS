using CleanArchitecture.Application.Common.Services.QrCode;
using QRCoder;

namespace CleanArchitecture.Infrastructure.QrCode
{
    public sealed class QrCodeService : IQrCodeService
    {
        public byte[] Generate(string content)
        {
            using var qrGenerator = new QRCodeGenerator();

            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}
