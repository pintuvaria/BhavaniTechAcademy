$content = Get-Content "src\BhavaniTech.UI\MainWindow.xaml.cs" -Raw
$content = $content -replace '(?s)private \(string Name, string Dob\) PromptForStudentRegistration\(\).*?private void LoadUserData\(\)\s*\{.*?\n\s*\}', $code
Set-Content "src\BhavaniTech.UI\MainWindow.xaml.cs" -Value $content
