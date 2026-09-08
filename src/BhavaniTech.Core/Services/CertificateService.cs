using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record CertificateRecord(
        string CertificateId,
        string StudentName,
        string TrackTitle,
        int CompletedLessons,
        int TotalScore,
        string IssueDate,
        string VerificationHash,
        string Issuer
    );

    public static class CertificateService
    {
        public static CertificateRecord CreateCertificateRecord(string studentName, string trackTitle, int completedLessons, int totalScore)
        {
            string issueDate = DateTime.UtcNow.ToString("MMMM dd, yyyy");
            string rawPayload = $"{studentName}|{trackTitle}|{completedLessons}|{totalScore}|{issueDate}|BhavaniTechDharmeshVaria";
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawPayload));
            string fullHash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
            string verificationHash = "BTA-" + fullHash.Substring(0, 12);
            string certId = "CERT-" + fullHash.Substring(12, 8);

            return new CertificateRecord(
                certId,
                string.IsNullOrWhiteSpace(studentName) ? "Dharmesh Varia" : studentName.Trim(),
                trackTitle,
                completedLessons,
                totalScore,
                issueDate,
                verificationHash,
                "Bhavani Technology — Founder: Dharmesh Varia"
            );
        }

        public static string GenerateCertificateHtml(CertificateRecord record)
        {
            return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<title>Bhavani Technology Certificate — {record.StudentName}</title>
<style>
  @page {{
    size: letter landscape;
    margin: 0;
  }}
  * {{
    box-sizing: border-box;
    margin: 0;
    padding: 0;
  }}
  body {{
    font-family: 'Georgia', serif;
    background: #0F172A;
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    padding: 20px;
    color: #1E293B;
  }}
  .cert-container {{
    background: #FFFFFF;
    width: 1000px;
    height: 700px;
    position: relative;
    padding: 40px;
    border: 12px solid #0F172A;
    box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
    background-image: radial-gradient(#F8FAFC 2px, transparent 2px);
    background-size: 32px 32px;
  }}
  .inner-border {{
    border: 3px double #D97706;
    height: 100%;
    padding: 30px;
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    position: relative;
  }}
  .watermark {{
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    font-size: 80px;
    font-weight: 900;
    color: rgba(15, 23, 42, 0.03);
    letter-spacing: 12px;
    pointer-events: none;
    font-family: 'Trebuchet MS', sans-serif;
  }}
  .logo {{
    font-family: 'Trebuchet MS', sans-serif;
    font-size: 24px;
    font-weight: 800;
    letter-spacing: 4px;
    color: #0F172A;
    text-transform: uppercase;
  }}
  .subtitle {{
    font-family: 'Trebuchet MS', sans-serif;
    font-size: 12px;
    font-weight: 600;
    letter-spacing: 3px;
    color: #D97706;
    text-transform: uppercase;
    margin-top: 4px;
  }}
  h1 {{
    font-size: 42px;
    font-weight: normal;
    color: #0F172A;
    margin: 25px 0 10px 0;
    letter-spacing: 2px;
  }}
  .presented-to {{
    font-style: italic;
    color: #64748B;
    font-size: 16px;
    margin-bottom: 8px;
  }}
  .student-name {{
    font-family: 'Times New Roman', serif;
    font-size: 38px;
    font-weight: bold;
    color: #0369A1;
    border-bottom: 2px solid #CBD5E1;
    padding-bottom: 4px;
    min-width: 450px;
    display: inline-block;
  }}
  .desc {{
    font-size: 16px;
    color: #334155;
    max-width: 700px;
    line-height: 1.6;
    margin-top: 15px;
  }}
  .track {{
    font-weight: bold;
    color: #0F172A;
  }}
  .footer {{
    width: 100%;
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
    margin-top: auto;
    padding-top: 20px;
  }}
  .sig-block {{
    text-align: center;
  }}
  .sig-line {{
    width: 200px;
    border-top: 1px solid #475569;
    margin-top: 30px;
    padding-top: 6px;
    font-size: 13px;
    color: #475569;
    font-family: 'Trebuchet MS', sans-serif;
  }}
  .seal {{
    width: 90px;
    height: 90px;
    border-radius: 50%;
    background: radial-gradient(circle, #F59E0B, #B45309);
    border: 3px solid #78350F;
    color: #FFF;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    font-family: 'Trebuchet MS', sans-serif;
    font-weight: bold;
    font-size: 9px;
    letter-spacing: 1px;
    text-transform: uppercase;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.2);
  }}
  .hash-box {{
    font-family: 'Courier New', monospace;
    font-size: 11px;
    color: #64748B;
    margin-top: 10px;
  }}
  .print-btn {{
    position: fixed;
    bottom: 20px;
    right: 20px;
    background: #0284C7;
    color: #FFF;
    border: none;
    padding: 12px 24px;
    font-size: 15px;
    font-weight: bold;
    border-radius: 8px;
    cursor: pointer;
    box-shadow: 0 4px 12px rgba(0,0,0,0.3);
    font-family: 'Trebuchet MS', sans-serif;
  }}
  .print-btn:hover {{
    background: #0369A1;
  }}
  @media print {{
    body {{
      background: none;
      padding: 0;
    }}
    .cert-container {{
      border: 8px solid #0F172A;
      box-shadow: none;
      width: 100%;
      height: 100vh;
    }}
    .print-btn {{
      display: none;
    }}
  }}
</style>
</head>
<body>
<div class=""cert-container"">
  <div class=""inner-border"">
    <div class=""watermark"">BHAVANI TECH</div>
    <div class=""logo"">Bhavani Technology</div>
    <div class=""subtitle"">Technology Learning &amp; Coding Academy</div>
    <h1>Certificate of Mastery</h1>
    <div class=""presented-to"">This certifies that</div>
    <div class=""student-name"">{record.StudentName}</div>
    <div class=""desc"">
      has demonstrated verifiable competence in <span class=""track"">{record.TrackTitle}</span>,
      successfully completing <span class=""track"">{record.CompletedLessons} hands-on laboratory modules</span>
      with an overall performance score of <span class=""track"">{record.TotalScore} XP</span> under the strict offline air-gapped curriculum.
    </div>
    <div class=""hash-box"">Verification Hash: <strong>{record.VerificationHash}</strong> | ID: <strong>{record.CertificateId}</strong></div>
    <div class=""footer"">
      <div class=""sig-block"">
        <div style=""font-family:'Brush Script MT', cursive; font-size: 26px; color: #0F172A;"">Dharmesh Varia</div>
        <div class=""sig-line"">Founder &amp; Chief Architect<br>Bhavani Technology</div>
      </div>
      <div class=""seal"">
        <span>★ OFFICIAL ★</span>
        <span style=""font-size: 11px; margin: 2px 0;"">ACADEMY</span>
        <span>VERIFIED</span>
      </div>
      <div class=""sig-block"">
        <div style=""font-size: 15px; font-weight: bold; color: #0F172A; padding-top: 10px;"">{record.IssueDate}</div>
        <div class=""sig-line"">Date of Issuance<br>100% Offline Certified</div>
      </div>
    </div>
  </div>
</div>
<button class=""print-btn"" onclick=""window.print()"">🖨️ Print Certificate (Save as PDF)</button>
</body>
</html>";
        }

        public static string ExportCertificateToFile(string studentName, string trackTitle, int completedLessons, int totalScore, string? outputDirectory = null)
        {
            var record = CreateCertificateRecord(studentName, trackTitle, completedLessons, totalScore);
            string html = GenerateCertificateHtml(record);

            string dir = outputDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates");
            Directory.CreateDirectory(dir);

            string safeName = string.Join("_", record.StudentName.Split(Path.GetInvalidFileNameChars()));
            string filePath = Path.Combine(dir, $"{safeName}_{record.CertificateId}.html");
            File.WriteAllText(filePath, html, Encoding.UTF8);

            return filePath;
        }
    }
}
