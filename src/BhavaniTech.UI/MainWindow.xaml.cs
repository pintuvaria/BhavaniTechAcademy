using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using BhavaniTech.Core.Database;
using BhavaniTech.Core.Hardware;
using BhavaniTech.Core.Models;
using BhavaniTech.Core.Services;

namespace BhavaniTech.UI
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseContext _db;
        private readonly DispatcherTimer _ramTimer;
        private List<Course> _allCourses = new();
        private List<Course> _courses = new();
        private List<Lesson> _activeLessons = new();
        private Course? _selectedCourse;
        private Lesson? _selectedLesson;
        private QuizQuestion? _currentQuiz;
        private List<CtfChallenge> _ctfChallenges = new();
        private User? _currentUser;
        private List<TroubleshootingSeeder.ScenarioRecord> _activeScenarios = new();
        private readonly VirtualTerminalSession _terminalSession = new();
        private List<DisassemblyPattern> _revEngPatterns = new();
        private readonly CalculatorEngine _calcEngine = new();
        private string _calcCurrentNumber = "0";
        private string _calcPendingOp = "";
        private double _calcFirstOperand = 0.0;
        private bool _calcIsNewEntry = true;
        private PracticalExam? _currentPracticalExam;

        // ARCADE GAMES STATE
        private MazeGameState? _currentMazeState;
        private List<NetworkPacket> _firewallPackets = new();
        private int _firewallScore = 0;
        private int _firewallWave = 1;
        private int _serverHealth = 100;
        private int _binaryTargetVal = 42;
        private bool[] _binaryBits = new bool[8];
        private int _binaryBlitzScore = 0;

        // ULTIMATE FEATURES STATE
        private readonly VisualGitSimulator _gitSim = new();
        private List<DebuggerFrameState> _debuggerFrames = new();
        private int _debuggerFrameIndex = 0;
        private List<Flashcard> _dueFlashcards = new();
        private int _currentFlashcardIndex = 0;
        private string _activeCheatCategory = "All Categories";
        private int _ctfActiveChallengeId = 1;

        public MainWindow()
        {
            InitializeComponent();
            _db = new DatabaseContext();

            _ramTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };
            _ramTimer.Tick += RamTimer_Tick;
            _ramTimer.Start();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Phase 8: Async UI Unblocking
            await System.Threading.Tasks.Task.Run(() =>
            {
                // Run heavy DB/initialization tasks in background
                Dispatcher.Invoke(() =>
                {
                    LoadUserData();
                    LoadCoursesData();
                    DetectHardwareProfile();
                    LoadCtfChallenges();
                    LoadRevEngPatterns();
                    UpdateDashboardHero();
                });
            });

            // Initialize Modern Polyglot Studio, Web Creator & Calculator Studio
            InitPolyglotStudio();
            InitWebStudio();
            InitCalculatorStudio();

            // Initialize Ultimate Systems & Labs
            InitStepDebugger();
            InitGitStudio();
            InitRestWorkbench();
            InitRegexLab();
            InitCheatSheets();
            InitAssemblyArena();
            InitWebcraftGame();
            InitCtfArenaGame();
            InitFlashcardsSRS();
            RefreshParentAudit();
            
            // Set initial chat response
            TxtAiChatHistory.Text = "AI Tutor: Hello Young Innovator! I am your offline AI learning assistant. Ask me questions about Python, C#, Networking, Cybersecurity, or Hardware!\n";
            TxtCyberAiResponse.Text = "Cyber AI Mentor: Welcome to the Cyber Ethical Hacking Lab! Click any of the study chips above or type your question below to learn Penetration Testing, Exploitation, and Defense.\n";
        }

        private void RamTimer_Tick(object? sender, EventArgs e)
        {
            double ramMB = SystemDiagnostics.GetCurrentMemoryUsageMB();
            TxtRamStatus.Text = $"RAM: {ramMB:F1} MB";
            
            if (ramMB < 150)
                TxtRamStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#4ADE80")!;
            else if (ramMB < 250)
                TxtRamStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#FACC15")!;
            else
                TxtRamStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444")!;
        }

        private void DetectHardwareProfile()
        {
            var sys = SystemDiagnostics.GetSystemCapabilities();
            TxtSettingsHardwareReport.Text = $"OS: {sys.OperatingSystem}\nCores: {sys.LogicalCores}\nTotal RAM: {sys.TotalRamMB} MB\nLow-End Profile: {(sys.IsLowEndHardware ? "ACTIVE (LOW-HARDWARE MODE AUTO-ENABLED)" : "NORMAL")}";
            
            if (sys.IsLowEndHardware)
            {
                TxtPerfStatus.Text = "Mode: LOW-HARDWARE";
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
                ChkSoftwareRender.IsChecked = true;
            }
        }

        private User PromptForUserSelection()
        {
            var users = _db.GetAllUsers();
            var window = new Window
            {
                Title = "Bhavani Tech Academy - Profile Selection",
                Width = 450,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Topmost = true,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"))
            };
            var sp = new StackPanel { Margin = new Thickness(20) };
            sp.Children.Add(new TextBlock { Text = "Welcome to Bhavani Technology", Foreground = Brushes.White, FontSize = 20, FontWeight = FontWeights.Bold, Margin = new Thickness(0,0,0,15), HorizontalAlignment = HorizontalAlignment.Center });
            
            var cboUsers = new ComboBox { FontSize = 16, Margin = new Thickness(0,0,0,15) };
            foreach (var u in users) { cboUsers.Items.Add(u.DisplayName + " (DOB: " + u.DateOfBirth + ")"); }
            if (users.Count > 0) cboUsers.SelectedIndex = 0;
            
            sp.Children.Add(new TextBlock { Text = "Select Existing Student:", Foreground = Brushes.LightGray, Margin = new Thickness(0,0,0,5) });
            sp.Children.Add(cboUsers);
            
            var btnLogin = new Button { Content = "Login", Height = 35, Margin = new Thickness(0,0,0,15) };
            sp.Children.Add(btnLogin);
            
            sp.Children.Add(new TextBlock { Text = "OR CREATE NEW STUDENT:", Foreground = Brushes.LightGray, Margin = new Thickness(0,10,0,5), HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeights.Bold });
            
            var tbName = new TextBox { FontSize = 16, Margin = new Thickness(0,0,0,10) };
            var tbDob = new TextBox { FontSize = 16, Margin = new Thickness(0,0,0,15) };
            sp.Children.Add(new TextBlock { Text = "Full Name:", Foreground = Brushes.LightGray, Margin = new Thickness(0,0,0,2) });
            sp.Children.Add(tbName);
            sp.Children.Add(new TextBlock { Text = "Date of Birth (MM/DD/YYYY):", Foreground = Brushes.LightGray, Margin = new Thickness(0,0,0,2) });
            sp.Children.Add(tbDob);
            
            var btnCreate = new Button { Content = "Register & Login", Height = 35 };
            sp.Children.Add(btnCreate);
            
            User selectedUser = null;
            
            btnLogin.Click += (s, e) => {
                if (cboUsers.SelectedIndex >= 0) {
                    selectedUser = users[cboUsers.SelectedIndex];
                    window.DialogResult = true;
                }
            };
            
            btnCreate.Click += (s, e) => {
                if (!string.IsNullOrWhiteSpace(tbName.Text) && !string.IsNullOrWhiteSpace(tbDob.Text)) {
                    _db.CreateStudent(tbName.Text.Trim(), tbDob.Text.Trim());
                    var newUsers = _db.GetAllUsers();
                    selectedUser = newUsers[^1];
                    window.DialogResult = true;
                }
            };
            
            window.Content = sp;
            window.ShowDialog();
            
            if (selectedUser != null) return selectedUser;
            if (users.Count > 0) return users[0];
            
            _db.CreateStudent("Student", "01/01/2000");
            return _db.GetAllUsers()[^1];
        }

        private void ShowCertificate(string name, string dob, string achievement)
        {
            // ── Professional Certificate Window ──────────────────────────────────────
            var win = new Window
            {
                Title = "Bhavani Technology Academy — Certificate of Excellence",
                Width = 900, Height = 640,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.CanMinimize,
                Topmost = true,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A"))
            };

            // Outer gold frame
            var outerBorder = new Border
            {
                Margin = new Thickness(14),
                BorderBrush = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                        new GradientStop((Color)ColorConverter.ConvertFromString("#D4AF37"), 0.0),
                        new GradientStop((Color)ColorConverter.ConvertFromString("#FFF8DC"), 0.5),
                        new GradientStop((Color)ColorConverter.ConvertFromString("#D4AF37"), 1.0)
                    }, 45),
                BorderThickness = new Thickness(8),
                CornerRadius = new CornerRadius(4),
                Background = Brushes.White
            };

            // Inner thin gold line
            var innerBorder = new Border
            {
                Margin = new Thickness(8),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37")),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(2)
            };

            // Main content grid
            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // header logo row
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // title
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // divider
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // presented to
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // name
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // dob
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // for mastering
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // achievement
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // spacer
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // footer divider
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // signatures

            // Row 0 — Logo / Academy name
            var logoPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0,10,0,6) };
            logoPanel.Children.Add(new TextBlock { Text = "⬡", FontSize = 40, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37")), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0,0,10,0) });
            var academyNamePanel = new StackPanel();
            academyNamePanel.Children.Add(new TextBlock { Text = "BHAVANI TECHNOLOGY", FontSize = 18, FontWeight = FontWeights.Black, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F")) });
            academyNamePanel.Children.Add(new TextBlock { Text = "A C A D E M Y", FontSize = 11, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37")) });
            logoPanel.Children.Add(academyNamePanel);
            Grid.SetRow(logoPanel, 0);
            grid.Children.Add(logoPanel);

            // Row 1 — CERTIFICATE OF EXCELLENCE
            var titleBlock = new TextBlock
            {
                Text = "CERTIFICATE OF EXCELLENCE",
                FontSize = 32, FontWeight = FontWeights.Black,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0,4,0,4)
            };
            Grid.SetRow(titleBlock, 1);
            grid.Children.Add(titleBlock);

            // Row 2 — Gold divider line
            var divider = new Rectangle
            {
                Height = 3, Margin = new Thickness(60,4,60,14),
                Fill = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                        new GradientStop(Colors.Transparent, 0.0),
                        new GradientStop((Color)ColorConverter.ConvertFromString("#D4AF37"), 0.3),
                        new GradientStop((Color)ColorConverter.ConvertFromString("#FFF8DC"), 0.5),
                        new GradientStop((Color)ColorConverter.ConvertFromString("#D4AF37"), 0.7),
                        new GradientStop(Colors.Transparent, 1.0)
                    }, 0)
            };
            Grid.SetRow(divider, 2);
            grid.Children.Add(divider);

            // Row 3 — "This is proudly presented to"
            var presentedTo = new TextBlock
            {
                Text = "This certificate is proudly presented to",
                FontSize = 16, FontStyle = FontStyles.Italic,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#475569")),
                HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0,0,0,4)
            };
            Grid.SetRow(presentedTo, 3);
            grid.Children.Add(presentedTo);

            // Row 4 — Student Name (large, elegant)
            var nameBlock = new TextBlock
            {
                Text = name,
                FontSize = 52, FontWeight = FontWeights.Bold, FontStyle = FontStyles.Italic,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F")),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0,0,0,2),
                FontFamily = new FontFamily("Georgia, Times New Roman, serif")
            };
            Grid.SetRow(nameBlock, 4);
            grid.Children.Add(nameBlock);

            // Row 5 — DOB
            var dobBlock = new TextBlock
            {
                Text = $"Date of Birth: {dob}",
                FontSize = 13,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8")),
                HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0,0,0,16)
            };
            Grid.SetRow(dobBlock, 5);
            grid.Children.Add(dobBlock);

            // Row 6 — "For mastering"
            var forBlock = new TextBlock
            {
                Text = "For successfully completing the examination and demonstrating mastery of:",
                FontSize = 15,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155")),
                HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0,0,0,6)
            };
            Grid.SetRow(forBlock, 6);
            grid.Children.Add(forBlock);

            // Row 7 — Achievement
            var achieveBorder = new Border
            {
                Margin = new Thickness(80,0,80,10),
                Padding = new Thickness(16,8,16,8),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F4E3")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37")),
                BorderThickness = new Thickness(1,1,1,1),
                CornerRadius = new CornerRadius(4)
            };
            var achieveBlock = new TextBlock
            {
                Text = achievement,
                FontSize = 20, FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7C2D12")),
                TextAlignment = TextAlignment.Center, TextWrapping = TextWrapping.Wrap
            };
            achieveBorder.Child = achieveBlock;
            Grid.SetRow(achieveBorder, 7);
            grid.Children.Add(achieveBorder);

            // Row 9 — Footer divider
            var divider2 = new Rectangle
            {
                Height = 1, Margin = new Thickness(40,8,40,8),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"))
            };
            Grid.SetRow(divider2, 9);
            grid.Children.Add(divider2);

            // Row 10 — Signatures row
            var sigGrid = new Grid { Margin = new Thickness(40,0,40,6) };
            sigGrid.ColumnDefinitions.Add(new ColumnDefinition());
            sigGrid.ColumnDefinitions.Add(new ColumnDefinition());
            sigGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Seal (centre)
            var sealPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            sealPanel.Children.Add(new TextBlock { Text = "✦ OFFICIAL SEAL ✦", FontSize = 11, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37")), HorizontalAlignment = HorizontalAlignment.Center });
            sealPanel.Children.Add(new Border { Width = 70, Height = 70, CornerRadius = new CornerRadius(35), BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37")), BorderThickness = new Thickness(3), Margin = new Thickness(0,4,0,4), Child = new TextBlock { Text = "BTA", FontSize = 22, FontWeight = FontWeights.Black, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F")), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } });
            Grid.SetColumn(sealPanel, 1);
            sigGrid.Children.Add(sealPanel);

            // Left signature
            var sig1 = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            sig1.Children.Add(new TextBlock { Text = "Dharmesh Varia", FontSize = 16, FontStyle = FontStyles.Italic, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F")), HorizontalAlignment = HorizontalAlignment.Center });
            sig1.Children.Add(new Rectangle { Height = 1, Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")), Margin = new Thickness(0,4,0,2) });
            sig1.Children.Add(new TextBlock { Text = "Founder & Chief Architect", FontSize = 10, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")), HorizontalAlignment = HorizontalAlignment.Center });
            sig1.Children.Add(new TextBlock { Text = "Bhavani Technology", FontSize = 10, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")), HorizontalAlignment = HorizontalAlignment.Center });
            Grid.SetColumn(sig1, 0);
            sigGrid.Children.Add(sig1);

            // Right — Issue info
            var issuePanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            issuePanel.Children.Add(new TextBlock { Text = $"Issued on", FontSize = 11, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")), HorizontalAlignment = HorizontalAlignment.Center });
            issuePanel.Children.Add(new TextBlock { Text = DateTime.Now.ToString("MMMM dd, yyyy"), FontSize = 15, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F")), HorizontalAlignment = HorizontalAlignment.Center });
            issuePanel.Children.Add(new Rectangle { Height = 1, Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")), Margin = new Thickness(0,4,0,2) });
            issuePanel.Children.Add(new TextBlock { Text = "Verified Offline Record", FontSize = 10, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")), HorizontalAlignment = HorizontalAlignment.Center });
            Grid.SetColumn(issuePanel, 2);
            sigGrid.Children.Add(issuePanel);

            Grid.SetRow(sigGrid, 10);
            grid.Children.Add(sigGrid);

            innerBorder.Child = grid;
            outerBorder.Child = innerBorder;
            win.Content = outerBorder;
            win.ShowDialog();
        }

        private void LoadUserData()
        {
            _currentUser = PromptForUserSelection();
            if (_currentUser != null)
            {
                TxtUserXp.Text = $"🚀 {_currentUser.TotalXP} XP";
                TxtUserLevel.Text = $"Lvl {_currentUser.CurrentLevel}";
                TxtUserStreak.Text = $"🔥 {_currentUser.CurrentStreak} Day Streak";
            }
        }

        private void LoadCoursesData()
        {
            _allCourses = _db.GetCourses();
            FilterCourses();
        }

        private void TxtCourseSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCourses();
        }

        private void BtnClearCourseSearch_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCourseSearch != null)
            {
                TxtCourseSearch.Text = "";
            }
        }

        private void FilterCourses()
        {
            if (LstCourses == null) return;

            string query = TxtCourseSearch?.Text?.Trim().ToLowerInvariant() ?? "";

            if (string.IsNullOrEmpty(query))
            {
                _courses = new List<Course>(_allCourses);
            }
            else
            {
                _courses = _allCourses.Where(c => 
                    c.Title.ToLowerInvariant().Contains(query) ||
                    c.Id.ToLowerInvariant().Contains(query) ||
                    c.Description.ToLowerInvariant().Contains(query) ||
                    c.Category.ToString().ToLowerInvariant().Contains(query)
                ).ToList();
            }

            LstCourses.ItemsSource = _courses.Select(c => $"[{c.Category}] {c.Title}").ToList();

            if (_courses.Count > 0)
            {
                LstCourses.SelectedIndex = 0;
            }
            else
            {
                _selectedCourse = null;
                LstLessons.ItemsSource = null;
                TxtLessonTitle.Text = "No Matching Courses";
                TxtLessonMeta.Text = $"Search query: '{query}' (0 matches)";
                TxtLessonContent.Text = $"No courses found matching '{query}'.\n\nTry searching for:\n• Python or C#\n• Cyber or Ethical Hacking\n• Linux or Terminal\n• Hardware or Microarchitecture\n• Networking or Subnetting\n• Electronics or Robotics\n• Artificial Intelligence or AI\n• Troubleshooting";
                if (TxtCourseDetailsTitle != null) TxtCourseDetailsTitle.Text = "No Course Selected";
                if (TxtCourseDetailsCategory != null) TxtCourseDetailsCategory.Text = "Category: None";
                if (TxtCoursePrerequisites != null) TxtCoursePrerequisites.Text = "Search for a valid course name to view prerequisites.";
                if (TxtCourseInstallationSteps != null) TxtCourseInstallationSteps.Text = "No course selected.";
                if (TxtCourseVerification != null) TxtCourseVerification.Text = "No verification commands.";
                if (TxtCourseToolsAndCareers != null) TxtCourseToolsAndCareers.Text = "None";
            }
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton btn && btn.Tag is string tag)
            {
                SwitchTab(tag);
            }
        }

        private void QuickLaunch_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                SwitchTab(tag);
            }
        }

        private void SwitchTab(string tag)
        {
            // Hide all top-level stations
            ViewDashboard.Visibility = Visibility.Collapsed;
            ViewCourses.Visibility = Visibility.Collapsed;
            if (ViewZeroToHero != null) ViewZeroToHero.Visibility = Visibility.Collapsed;
            if (ViewGaming != null) ViewGaming.Visibility = Visibility.Collapsed;
            ViewLabs.Visibility = Visibility.Collapsed;
            ViewMastery.Visibility = Visibility.Collapsed;

            // Show selected station and configure active workbench
            switch (tag)
            {
                case "Dashboard":
                    ViewDashboard.Visibility = Visibility.Visible;
                    TxtActiveTabTitle.Text = "Student Learning HQ & Syllabus";
                    if (NavDashboard != null) NavDashboard.IsChecked = true;
                    UpdateDashboardHero();
                    break;

                case "Courses":
                    ViewCourses.Visibility = Visibility.Visible;
                    TxtActiveTabTitle.Text = "Active Study Hall (Curriculum & Mastery Workflow)";
                    if (NavCourses != null) NavCourses.IsChecked = true;
                    break;

                case "ZeroToHero":
                    if (ViewZeroToHero != null) ViewZeroToHero.Visibility = Visibility.Visible;
                    TxtActiveTabTitle.Text = "Zero to Hero: Complete Technology Mastery (Roadmap, Diagnostic & Capstones)";
                    if (NavZeroToHero != null) NavZeroToHero.IsChecked = true;
                    InitZeroToHero();
                    break;

                case "Gaming":
                    if (ViewGaming != null) ViewGaming.Visibility = Visibility.Visible;
                    TxtActiveTabTitle.Text = "Learn with Gaming Arcade (5 Interactive Tech Games)";
                    if (NavGaming != null) NavGaming.IsChecked = true;
                    InitArcadeGames();
                    break;

                case "Labs":
                    ViewLabs.Visibility = Visibility.Visible;
                    TxtActiveTabTitle.Text = "Creative Workbenches & Project Studios";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    break;

                case "Programming":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 0;
                    TxtActiveTabTitle.Text = "Creative Workbenches: Polyglot Code IDE";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    break;

                case "Software":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 1;
                    TxtActiveTabTitle.Text = "Creative Workbenches: Software Engineering & OS Internals";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    break;

                case "Hardware":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 2;
                    TxtActiveTabTitle.Text = "Creative Workbenches: 2D Virtual PC Builder";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    break;

                case "Networking":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 3;
                    TxtActiveTabTitle.Text = "Creative Workbenches: Networking Academy & Topology";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    break;

                case "Cybersecurity":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 4;
                    TxtActiveTabTitle.Text = "Creative Workbenches: Cyber Ethical Hacking Labs";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    break;

                case "Ai":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 5;
                    TxtActiveTabTitle.Text = "Creative Workbenches: Frontier AI & Neural Networks";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    break;

                case "Electronics":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 6;
                    TxtActiveTabTitle.Text = "Creative Workbenches: Electronics & Arduino Circuits";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    break;

                case "Linux":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 7;
                    TxtActiveTabTitle.Text = "Creative Workbenches: Linux Terminal Simulator";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    InitLinuxTerminal();
                    break;

                case "Troubleshooting":
                    ViewLabs.Visibility = Visibility.Visible;
                    if (TabsCreativeLabs != null) TabsCreativeLabs.SelectedIndex = 8;
                    TxtActiveTabTitle.Text = "Creative Workbenches: IT Troubleshooting (500+ Scenarios)";
                    if (NavLabs != null) NavLabs.IsChecked = true;
                    LoadTroubleshootingScenarios();
                    break;

                case "Mastery":
                    ViewMastery.Visibility = Visibility.Visible;
                    if (TabsMastery != null) TabsMastery.SelectedIndex = 0;
                    TxtActiveTabTitle.Text = "Student Progress, Badges & Verifiable Diploma";
                    if (NavMastery != null) NavMastery.IsChecked = true;
                    RefreshBadgesList();
                    break;

                case "Settings":
                    ViewMastery.Visibility = Visibility.Visible;
                    if (TabsMastery != null) TabsMastery.SelectedIndex = 3;
                    TxtActiveTabTitle.Text = "Mastery Station: Low-Hardware & Performance Settings";
                    if (NavMastery != null) NavMastery.IsChecked = true;
                    break;
            }

            // Aggressive memory trim on view change if performance mode enabled
            if (ChkAggressiveMemory?.IsChecked == true)
            {
                SystemDiagnostics.OptimizeMemoryUsage();
            }
        }

        public void UpdateDashboardHero()
        {
            try
            {
                var completedIds = _db.GetCompletedLessonIds(1);
                int totalLessons = _allCourses.Sum(c => _db.GetLessonsForCourse(c.Id).Count);
                if (totalLessons == 0) totalLessons = 66;
                int completedCount = completedIds.Count;
                int pct = (int)Math.Round((double)completedCount / totalLessons * 100);
                if (pct > 100) pct = 100;

                if (PbHeroProgress != null) PbHeroProgress.Value = pct;
                if (TxtHeroProgressPct != null)
                    TxtHeroProgressPct.Text = $"Curriculum Progress: {pct}% Complete ({completedCount} of {totalLessons} Lessons Finished)";
                if (TxtMetricLessonsDone != null)
                    TxtMetricLessonsDone.Text = $"{completedCount} / {totalLessons} Lessons";

                var (recCourse, recLesson) = _db.GetNextRecommendedLesson(1);
                if (recCourse != null && recLesson != null)
                {
                    if (TxtHeroCourseTitle != null)
                        TxtHeroCourseTitle.Text = $"Active Track: [{recCourse.Category}] {recCourse.Title}";
                    if (TxtHeroLessonTitle != null)
                        TxtHeroLessonTitle.Text = $"Next Milestone: {recLesson.Title} (Lvl: {recLesson.Difficulty})";
                }
                else
                {
                    if (TxtHeroCourseTitle != null)
                        TxtHeroCourseTitle.Text = "All Core Tracks Completed!";
                    if (TxtHeroLessonTitle != null)
                        TxtHeroLessonTitle.Text = "Ready for Comprehensive Certification Exam in Progress & Diploma station.";
                }

                if (_currentUser != null)
                {
                    if (TxtHeroGreeting != null)
                        TxtHeroGreeting.Text = $"Welcome back, {_currentUser.Username}! Ready to learn today?";
                    if (TxtHeroStreakBadge != null)
                        TxtHeroStreakBadge.Text = $"🔥 Active Streak: {_currentUser.CurrentStreak} Day{(_currentUser.CurrentStreak == 1 ? "" : "s")}";
                }
            }
            catch
            {
                // Safe fallback
            }
        }

        private void BtnHeroContinue_Click(object sender, RoutedEventArgs e)
        {
            var (course, lesson) = _db.GetNextRecommendedLesson(1);
            NavCourses.IsChecked = true;
            SwitchTab("Courses");
            if (course != null && lesson != null)
            {
                SelectCourseAndLesson(course.Id, lesson.Id);
            }
        }

        private void BtnHeroExploreLabs_Click(object sender, RoutedEventArgs e)
        {
            NavLabs.IsChecked = true;
            SwitchTab("Labs");
        }

        private void TrackSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string trackTag)
            {
                if (trackTag == "MAST101")
                {
                    NavMastery.IsChecked = true;
                    SwitchTab("Mastery");
                    return;
                }

                NavCourses.IsChecked = true;
                SwitchTab("Courses");

                string targetCourseId = trackTag switch
                {
                    "CS101" => "CS101",
                    "PROG101" => "PROG101",
                    "LNX101" => "LNX101",
                    "SEC101" => "SEC101",
                    "AI101" => "AI101",
                    _ => "CS101"
                };

                var matchedCourse = _allCourses.FirstOrDefault(c => c.Id.Equals(targetCourseId, StringComparison.OrdinalIgnoreCase))
                                 ?? _allCourses.FirstOrDefault();

                if (matchedCourse != null)
                {
                    SelectCourseAndLesson(matchedCourse.Id);
                }
            }
        }

        private void BtnLaunchLessonLab_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCourse == null)
            {
                NavLabs.IsChecked = true;
                SwitchTab("Labs");
                return;
            }

            NavLabs.IsChecked = true;
            switch (_selectedCourse.Category)
            {
                case CourseCategory.Programming:
                    SwitchTab("Programming");
                    break;
                case CourseCategory.Cybersecurity:
                    SwitchTab("Cybersecurity");
                    break;
                case CourseCategory.Linux:
                    SwitchTab("Linux");
                    break;
                case CourseCategory.ArtificialIntelligence:
                    SwitchTab("Ai");
                    break;
                case CourseCategory.Electronics:
                    SwitchTab("Electronics");
                    break;
                case CourseCategory.Hardware:
                    SwitchTab("Hardware");
                    break;
                case CourseCategory.Networking:
                    SwitchTab("Networking");
                    break;
                case CourseCategory.Fundamentals:
                    SwitchTab("Software");
                    break;
                case CourseCategory.Troubleshooting:
                    SwitchTab("Troubleshooting");
                    break;
                default:
                    SwitchTab("Labs");
                    break;
            }
        }

        private void SelectCourseAndLesson(string courseId, string? lessonId = null)
        {
            if (TxtCourseSearch != null) TxtCourseSearch.Text = "";
            _courses = new List<Course>(_allCourses);
            if (LstCourses != null)
            {
                LstCourses.ItemsSource = _courses.Select(c => $"[{c.Category}] {c.Title}").ToList();
                int courseIdx = _courses.FindIndex(c => c.Id.Equals(courseId, StringComparison.OrdinalIgnoreCase));
                if (courseIdx >= 0)
                {
                    LstCourses.SelectedIndex = courseIdx;
                    if (!string.IsNullOrEmpty(lessonId) && _activeLessons != null && LstLessons != null)
                    {
                        int lessonIdx = _activeLessons.FindIndex(l => l.Id.Equals(lessonId, StringComparison.OrdinalIgnoreCase));
                        if (lessonIdx >= 0)
                        {
                            LstLessons.SelectedIndex = lessonIdx;
                        }
                    }
                }
            }
        }

        private void BtnTrimRam_Click(object sender, RoutedEventArgs e)
        {
            double before = SystemDiagnostics.GetCurrentMemoryUsageMB();
            SystemDiagnostics.OptimizeMemoryUsage();
            double after = SystemDiagnostics.GetCurrentMemoryUsageMB();
            MessageBox.Show($"RAM Trimmed successfully!\nFreed: {(before - after):F1} MB", "Low Hardware Engine", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
        }

        // COURSES & LESSONS (BASICS TO MASTERS)
        private void LstCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstCourses == null || LstLessons == null) return;
            int idx = LstCourses.SelectedIndex;
            if (idx >= 0 && idx < _courses.Count)
            {
                _selectedCourse = _courses[idx];
                UpdateCourseDetailsView(_selectedCourse.Id);
                FilterLessons();
            }
        }

        private void UpdateCourseDetailsView(string courseId)
        {
            var details = CourseDetailsProvider.GetDetails(courseId);
            if (TxtCourseDetailsTitle != null) TxtCourseDetailsTitle.Text = $"🛠️ {details.CourseTitle} (Code: {details.CourseId})";
            if (TxtCourseDetailsCategory != null) TxtCourseDetailsCategory.Text = $"Category: {details.Category} | Hardware: {details.HardwareRequirements.Replace("\n", "  •  ")}";
            if (TxtCoursePrerequisites != null) TxtCoursePrerequisites.Text = details.Prerequisites;
            if (TxtCourseInstallationSteps != null) TxtCourseInstallationSteps.Text = details.InstallationSteps;
            if (TxtCourseVerification != null) TxtCourseVerification.Text = details.EnvironmentVerification;
            if (TxtCourseToolsAndCareers != null) TxtCourseToolsAndCareers.Text = $"Recommended Tools:\n{details.RecommendedTools}\n\nCareer Pathways & Certifications:\n{details.CareerAndCertifications}";
        }

        private void CmbLessonDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterLessons();
        }

        private void FilterLessons()
        {
            if (_selectedCourse == null || LstLessons == null) return;

            string diff = (CmbLessonDifficulty?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "All Levels";
            _activeLessons = _db.GetLessonsForCourse(_selectedCourse.Id, diff);
            LstLessons.ItemsSource = _activeLessons.Select(l => $"[{l.Difficulty}] {l.Title}").ToList();

            if (_activeLessons.Count > 0)
            {
                LstLessons.SelectedIndex = 0;
            }
            else
            {
                TxtLessonTitle.Text = _selectedCourse.Title;
                TxtLessonMeta.Text = $"No lessons found for difficulty filter: {diff}";
                TxtLessonContent.Text = "Select another mastery level filter to explore lessons in this course.";
                TxtQuizQuestion.Text = "No quiz available for this level.";
                BrdQuizFeedback.Visibility = Visibility.Collapsed;
            }
        }

        private void LstLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstLessons == null || TxtLessonTitle == null || TxtLessonMeta == null || TxtLessonContent == null) return;

            int idx = LstLessons.SelectedIndex;
            if (idx >= 0 && idx < _activeLessons.Count)
            {
                _selectedLesson = _activeLessons[idx];
                TxtLessonTitle.Text = _selectedLesson.Title;
                TxtLessonMeta.Text = $"Duration: {_selectedLesson.EstimatedMinutes} mins | Difficulty: {_selectedLesson.Difficulty}";

                // Enrich lesson content with course blueprint, prerequisites, and quick setup banner
                var courseDetails = _selectedCourse != null ? CourseDetailsProvider.GetDetails(_selectedCourse.Id) : null;
                var sb = new System.Text.StringBuilder();
                if (courseDetails != null)
                {
                    sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
                    sb.AppendLine($"  COURSE: {courseDetails.CourseTitle} [{courseDetails.Category}]");
                    sb.AppendLine($"  PREREQUISITES: {courseDetails.Prerequisites.Replace("\n", "  |  ")}");
                    sb.AppendLine($"  SETUP GUIDE: See 'Course Blueprint & Setup' tab for full CLI instructions");
                    sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════════╝\n");
                }
                sb.AppendLine(_selectedLesson.ContentMarkdown);

                TxtLessonContent.Text = sb.ToString();

                LoadLessonQuiz(_selectedLesson.Id);
                LoadPracticalExam(_selectedLesson.Id);
            }
        }

        private void LoadLessonQuiz(string lessonId)
        {
            _currentQuiz = _db.GetQuizForLesson(lessonId);
            if (_currentQuiz != null)
            {
                TxtQuizQuestion.Text = _currentQuiz.QuestionText;
                RbOptionA.Content = $"A) {_currentQuiz.OptionA}";
                RbOptionB.Content = $"B) {_currentQuiz.OptionB}";
                RbOptionC.Content = $"C) {_currentQuiz.OptionC}";
                RbOptionD.Content = $"D) {_currentQuiz.OptionD}";
                RbOptionA.IsChecked = false;
                RbOptionB.IsChecked = false;
                RbOptionC.IsChecked = false;
                RbOptionD.IsChecked = false;
                BrdQuizFeedback.Visibility = Visibility.Collapsed;
            }
            else
            {
                TxtQuizQuestion.Text = "Interactive quiz coming soon for this lesson.";
                RbOptionA.Content = "Option A";
                RbOptionB.Content = "Option B";
                RbOptionC.Content = "Option C";
                RbOptionD.Content = "Option D";
                BrdQuizFeedback.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnSubmitQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuiz == null)
            {
                MessageBox.Show("Please select a lesson with an active quiz first.", "Quiz", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
                return;
            }

            char selected = ' ';
            if (RbOptionA.IsChecked == true) selected = 'A';
            else if (RbOptionB.IsChecked == true) selected = 'B';
            else if (RbOptionC.IsChecked == true) selected = 'C';
            else if (RbOptionD.IsChecked == true) selected = 'D';

            if (selected == ' ')
            {
                MessageBox.Show("Please select an option (A, B, C, or D) before submitting.", "Quiz", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool correct = char.ToUpperInvariant(selected) == char.ToUpperInvariant(_currentQuiz.CorrectOption);
            BrdQuizFeedback.Visibility = Visibility.Visible;

            if (correct)
            {
                int totalXp = _db.AddUserXp(30);
                TxtUserXp.Text = $"⭐ {totalXp} XP";
                TxtQuizFeedback.Foreground = (Brush)new BrushConverter().ConvertFrom("#4ADE80")!;
                TxtQuizFeedback.Text = $"✅ CORRECT! (+30 XP Earned)\nYour Answer: Option {selected}\n\nEXPLANATION:\n{_currentQuiz.Explanation}";
            }
            else
            {
                TxtQuizFeedback.Foreground = (Brush)new BrushConverter().ConvertFrom("#F87171")!;
                TxtQuizFeedback.Text = $"❌ INCORRECT!\nYour Answer: Option {selected} | Correct Answer: Option {_currentQuiz.CorrectOption}\n\nEXPLANATION:\n{_currentQuiz.Explanation}";
            }
        }

        private void BtnCompleteLesson_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLesson != null)
            {
                _db.MarkLessonCompleted(_selectedLesson.Id, 1);
                int totalXp = _db.AddUserXp(50);
                TxtUserXp.Text = $"⭐ {totalXp} XP";
                LoadUserData();
                UpdateDashboardHero();

                MessageBox.Show(
                    $"🎉 Milestone Achieved! (+50 XP)\n\n" +
                    $"Lesson: {_selectedLesson.Title}\n" +
                    $"Course: {_selectedCourse?.Title ?? "Bhavani Academy"}\n\n" +
                    $"Your progress is saved into your permanent offline record.\n" +
                    $"Advancing to the next curriculum milestone...",
                    "Milestone Complete! 🏆",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);

                // Auto-advance to next lesson in the current course or next recommended track
                if (LstLessons != null && LstLessons.Items.Count > 0)
                {
                    if (LstLessons.SelectedIndex < LstLessons.Items.Count - 1)
                    {
                        LstLessons.SelectedIndex++;
                    }
                    else
                    {
                        var (nextCourse, nextLesson) = _db.GetNextRecommendedLesson(1);
                        if (nextCourse != null && nextLesson != null)
                        {
                            SelectCourseAndLesson(nextCourse.Id, nextLesson.Id);
                        }
                    }
                }
            }
        }

        // =====================================================================
        // POLYGLOT CODE IDE (10 LANGUAGES WITH VISUAL LOGOS & BADGES)
        // =====================================================================
        private void InitPolyglotStudio()
        {
            UpdateLanguageChip("python", loadTemplate: true);
        }

        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbLanguage?.SelectedItem is ComboBoxItem item)
            {
                string content = item.Content.ToString() ?? "Python";
                string langKey = "python";
                if (content.Contains("C#")) langKey = "csharp";
                else if (content.Contains("JavaScript")) langKey = "javascript";
                else if (content.Contains("HTML")) langKey = "htmlcss";
                else if (content.Contains("SQL")) langKey = "sql";
                else if (content.Contains("C++")) langKey = "cpp";
                else if (content.Contains("Rust")) langKey = "rust";
                else if (content.Contains("Java") && !content.Contains("Script")) langKey = "java";
                else if (content.Contains("Go")) langKey = "go";
                else if (content.Contains("Assembly")) langKey = "assembly";

                UpdateLanguageChip(langKey, loadTemplate: true);
            }
        }

        private void UpdateLanguageChip(string langKey, bool loadTemplate = false)
        {
            var spec = LanguageRegistryService.GetLanguage(langKey);
            if (TxtLangLogo != null) TxtLangLogo.Text = spec.LogoEmoji;
            if (TxtLangTitle != null) TxtLangTitle.Text = spec.Version;
            if (TxtLangParadigm != null) TxtLangParadigm.Text = spec.Paradigm;

            if (BrdLangChip != null)
            {
                try
                {
                    var brandBrush = (Brush)new BrushConverter().ConvertFrom(spec.BrandHexColor)!;
                    BrdLangChip.BorderBrush = brandBrush;
                    if (TxtLangTitle != null) TxtLangTitle.Foreground = brandBrush;
                }
                catch { }
            }

            if (loadTemplate && TxtCodeInput != null)
            {
                TxtCodeInput.Text = spec.StarterTemplateCode;
            }
        }

        private void BtnLoadTemplate_Click(object sender, RoutedEventArgs e)
        {
            string content = (CmbLanguage?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Python";
            string langKey = "python";
            if (content.Contains("C#")) langKey = "csharp";
            else if (content.Contains("JavaScript")) langKey = "javascript";
            else if (content.Contains("HTML")) langKey = "htmlcss";
            else if (content.Contains("SQL")) langKey = "sql";
            else if (content.Contains("C++")) langKey = "cpp";
            else if (content.Contains("Rust")) langKey = "rust";
            else if (content.Contains("Java") && !content.Contains("Script")) langKey = "java";
            else if (content.Contains("Go")) langKey = "go";
            else if (content.Contains("Assembly")) langKey = "assembly";

            var spec = LanguageRegistryService.GetLanguage(langKey);
            if (TxtCodeInput != null) TxtCodeInput.Text = spec.StarterTemplateCode;
        }

        private void BtnClearCode_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCodeInput != null) TxtCodeInput.Text = "";
            if (TxtCodeOutput != null) TxtCodeOutput.Text = "[Editor cleared. Ready to type or load templates.]";
        }

        private void BtnRunCode_Click(object sender, RoutedEventArgs e)
        {
            string content = (CmbLanguage?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Python";
            string langKey = "python";
            if (content.Contains("C#")) langKey = "csharp";
            else if (content.Contains("JavaScript")) langKey = "javascript";
            else if (content.Contains("HTML")) langKey = "htmlcss";
            else if (content.Contains("SQL")) langKey = "sql";
            else if (content.Contains("C++")) langKey = "cpp";
            else if (content.Contains("Rust")) langKey = "rust";
            else if (content.Contains("Java") && !content.Contains("Script")) langKey = "java";
            else if (content.Contains("Go")) langKey = "go";
            else if (content.Contains("Assembly")) langKey = "assembly";

            string code = TxtCodeInput.Text;
            var res = CodeExecutionService.ExecuteCode(langKey, code);
            var spec = LanguageRegistryService.GetLanguage(langKey);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== {spec.LogoEmoji} {spec.DisplayName.ToUpperInvariant()} EXECUTION SANDBOX ({res.ExecutionTimeMs:F1} ms) ===");
            sb.AppendLine(res.Output);
            if (!string.IsNullOrEmpty(res.Error))
            {
                sb.AppendLine($"\n[ERROR]: {res.Error}");
            }
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine($"Language: {spec.Version} | Paradigm: {spec.Paradigm}");
            TxtCodeOutput.Text = sb.ToString();
        }

        // =====================================================================
        // WEB CREATOR & LIVE WEBSITE PREVIEW
        // =====================================================================
        // WEB STUDIO & OFFLINE LIVE DOM BROWSER PREVIEWER
        // =====================================================================
        private void InitWebStudio()
        {
            EnsureBrowserEmulation();
            if (WbLivePreview != null)
            {
                WbLivePreview.Navigated += (s, e) => SuppressScriptErrors(WbLivePreview);
            }
            LoadWebTemplate(0);
        }

        private static void EnsureBrowserEmulation()
        {
            try
            {
                string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
                using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                    true);
                key?.SetValue(processName, 11001, Microsoft.Win32.RegistryValueKind.DWord);
                key?.SetValue("BhavaniTech.UI.exe", 11001, Microsoft.Win32.RegistryValueKind.DWord);
            }
            catch
            {
                // Best effort
            }
        }

        private static void SuppressScriptErrors(WebBrowser? wb)
        {
            if (wb == null) return;
            try
            {
                var axProp = typeof(WebBrowser).GetProperty("AxIWebBrowser2", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var ax = axProp?.GetValue(wb);
                if (ax != null)
                {
                    ax.GetType().InvokeMember("Silent", 
                        System.Reflection.BindingFlags.SetProperty, 
                        null, ax, new object[] { true });
                }
            }
            catch
            {
                // Best effort
            }
        }

        private void CmbWebTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbWebTemplate != null)
            {
                LoadWebTemplate(CmbWebTemplate.SelectedIndex);
            }
        }

        private void LoadWebTemplate(int idx)
        {
            if (TxtWebHtmlInput == null) return;

            switch (idx)
            {
                case 1: // Modern Developer Portfolio Card
                    TxtWebHtmlInput.Text = 
                        "<!DOCTYPE html>\n" +
                        "<html>\n" +
                        "<head>\n" +
                        "  <meta charset=\"UTF-8\">\n" +
                        "  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n" +
                        "  <style>\n" +
                        "    body { font-family: 'Segoe UI', sans-serif; background: #0F172A; color: #F8FAFC; padding: 30px; margin: 0; }\n" +
                        "    .portfolio-card { background: #1E293B; border: 1px solid #334155; border-radius: 16px; padding: 24px; max-width: 480px; margin: auto; box-shadow: 0 20px 30px rgba(0,0,0,0.6); }\n" +
                        "    .avatar { width: 70px; height: 70px; border-radius: 50%; background: linear-gradient(135deg, #38BDF8, #818CF8); display: flex; align-items: center; justify-content: center; font-size: 32px; margin-bottom: 12px; }\n" +
                        "    h2 { margin: 0 0 4px 0; color: #38BDF8; font-size: 22px; }\n" +
                        "    .tagline { color: #94A3B8; font-size: 13px; margin-bottom: 16px; }\n" +
                        "    .badges { display: flex; flex-wrap: wrap; gap: 6px; margin-bottom: 20px; }\n" +
                        "    .badge { background: #020617; border: 1px solid #38BDF8; color: #38BDF8; padding: 4px 10px; border-radius: 20px; font-size: 11px; font-weight: bold; }\n" +
                        "    .stats { display: flex; justify-content: space-around; background: #0F172A; border-radius: 10px; padding: 12px; margin-bottom: 20px; }\n" +
                        "    .stat-val { font-size: 18px; font-weight: bold; color: #4ADE80; }\n" +
                        "    .stat-label { font-size: 10px; color: #94A3B8; }\n" +
                        "    .btn-hire { background: #0284C7; color: white; border: none; width: 100%; padding: 12px; border-radius: 8px; font-size: 14px; font-weight: bold; cursor: pointer; transition: 0.2s; }\n" +
                        "    .btn-hire:hover { background: #0369A1; }\n" +
                        "    #contactMsg { display: none; margin-top: 12px; padding: 10px; background: #064E3B; border-radius: 6px; color: #34D399; font-size: 12px; text-align: center; }\n" +
                        "  </style>\n" +
                        "</head>\n" +
                        "<body>\n" +
                        "  <div class=\"portfolio-card\">\n" +
                        "    <div class=\"avatar\">👨‍💻</div>\n" +
                        "    <h2>Dharmesh Varia</h2>\n" +
                        "    <div class=\"tagline\">Founder & Chief Architect • Full-Stack & Cyber Engineer</div>\n" +
                        "    <div class=\"badges\">\n" +
                        "      <span class=\"badge\">C# .NET 9</span>\n" +
                        "      <span class=\"badge\">Python 3.12</span>\n" +
                        "      <span class=\"badge\">Cybersecurity</span>\n" +
                        "      <span class=\"badge\">Local AI RAG</span>\n" +
                        "      <span class=\"badge\">Microarchitecture</span>\n" +
                        "    </div>\n" +
                        "    <div class=\"stats\">\n" +
                        "      <div><div class=\"stat-val\">66</div><div class=\"stat-label\">Lessons Built</div></div>\n" +
                        "      <div><div class=\"stat-val\">100%</div><div class=\"stat-label\">Offline Native</div></div>\n" +
                        "      <div><div class=\"stat-val\">42 MB</div><div class=\"stat-label\">RAM Footprint</div></div>\n" +
                        "    </div>\n" +
                        "    <button class=\"btn-hire\" onclick=\"showMessage()\">📬 Connect & Hire Me</button>\n" +
                        "    <div id=\"contactMsg\">✅ Thank you for connecting! Bhavani Tech project portfolio ready to deploy.</div>\n" +
                        "  </div>\n" +
                        "  <script>\n" +
                        "    function showMessage() {\n" +
                        "      document.getElementById('contactMsg').style.display = 'block';\n" +
                        "    }\n" +
                        "  </script>\n" +
                        "</body>\n" +
                        "</html>\n";
                    break;

                case 2: // Cyber Security Cyberpunk Terminal
                    TxtWebHtmlInput.Text = 
                        "<!DOCTYPE html>\n" +
                        "<html>\n" +
                        "<head>\n" +
                        "  <meta charset=\"UTF-8\">\n" +
                        "  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n" +
                        "  <style>\n" +
                        "    body { font-family: 'Consolas', monospace; background: #000; color: #00FF66; padding: 20px; margin: 0; }\n" +
                        "    .terminal-box { border: 2px solid #00FF66; border-radius: 8px; padding: 18px; box-shadow: 0 0 20px rgba(0, 255, 102, 0.4); max-width: 550px; margin: auto; }\n" +
                        "    .header { border-bottom: 1px solid #00FF66; padding-bottom: 8px; margin-bottom: 12px; display: flex; justify-content: space-between; font-size: 12px; }\n" +
                        "    .feed { height: 180px; overflow-y: auto; font-size: 13px; line-height: 1.5; }\n" +
                        "    .feed p { margin: 4px 0; }\n" +
                        "    .scan-btn { background: #00FF66; color: #000; border: none; font-weight: bold; padding: 8px 16px; border-radius: 4px; cursor: pointer; margin-top: 12px; font-family: monospace; }\n" +
                        "    .scan-btn:hover { background: #66FFAA; }\n" +
                        "  </style>\n" +
                        "</head>\n" +
                        "<body>\n" +
                        "  <div class=\"terminal-box\">\n" +
                        "    <div class=\"header\">\n" +
                        "      <span>BHAVANI CYBER SECURITY SENTINEL v4.1</span>\n" +
                        "      <span id=\"clock\">00:00:00</span>\n" +
                        "    </div>\n" +
                        "    <div class=\"feed\" id=\"logFeed\">\n" +
                        "      <p>[SYSTEM] Kernel security monitor initialized...</p>\n" +
                        "      <p>[OK] All offline firewalls active and enforcing.</p>\n" +
                        "      <p>[INFO] Subnet 192.168.1.0/24 ready for probe.</p>\n" +
                        "    </div>\n" +
                        "    <button class=\"scan-btn\" onclick=\"runScan()\">[▶ EXECUTE RECON SCAN]</button>\n" +
                        "  </div>\n" +
                        "  <script>\n" +
                        "    setInterval(function() { document.getElementById('clock').innerText = new Date().toLocaleTimeString(); }, 1000);\n" +
                        "    var scanCount = 0;\n" +
                        "    function runScan() {\n" +
                        "      scanCount++;\n" +
                        "      var feed = document.getElementById('logFeed');\n" +
                        "      var ports = [22, 80, 443, 8080, 53];\n" +
                        "      var targetPort = ports[Math.floor(Math.random() * ports.length)];\n" +
                        "      feed.innerHTML += '<p>[+] Port ' + targetPort + '/TCP OPEN — Service Banner: Apache/2.4 (Probe #' + scanCount + ')</p>';\n" +
                        "      feed.scrollTop = feed.scrollHeight;\n" +
                        "    }\n" +
                        "  </script>\n" +
                        "</body>\n" +
                        "</html>\n";
                    break;

                case 3: // CSS3 Flexbox Grid Product Showcase
                    TxtWebHtmlInput.Text = 
                        "<!DOCTYPE html>\n" +
                        "<html>\n" +
                        "<head>\n" +
                        "  <meta charset=\"UTF-8\">\n" +
                        "  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n" +
                        "  <style>\n" +
                        "    body { font-family: 'Segoe UI', sans-serif; background: #0F172A; color: #F8FAFC; padding: 24px; margin: 0; }\n" +
                        "    .store-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; max-width: 600px; margin: auto; }\n" +
                        "    .item-card { background: #1E293B; border: 1px solid #334155; border-radius: 12px; padding: 16px; text-align: center; }\n" +
                        "    .item-icon { font-size: 38px; margin-bottom: 8px; }\n" +
                        "    .item-title { font-weight: bold; font-size: 14px; margin-bottom: 4px; }\n" +
                        "    .item-price { color: #38BDF8; font-weight: bold; font-size: 16px; margin-bottom: 10px; }\n" +
                        "    .btn-buy { background: #10B981; color: white; border: none; padding: 8px 14px; border-radius: 6px; font-weight: bold; cursor: pointer; width: 100%; }\n" +
                        "    .btn-buy:hover { background: #059669; }\n" +
                        "    .cart-summary { max-width: 600px; margin: 16px auto; background: #020617; border: 1px solid #334155; border-radius: 8px; padding: 12px; display: flex; justify-content: space-between; align-items: center; }\n" +
                        "    .cart-total { font-size: 16px; font-weight: bold; color: #FACC15; }\n" +
                        "  </style>\n" +
                        "</head>\n" +
                        "<body>\n" +
                        "  <div class=\"cart-summary\">\n" +
                        "    <span>🛒 Shopping Cart Items: <strong id=\"cartItems\">0</strong></span>\n" +
                        "    <span class=\"cart-total\">Total: $<span id=\"cartPrice\">0.00</span></span>\n" +
                        "  </div>\n" +
                        "  <div class=\"store-grid\">\n" +
                        "    <div class=\"item-card\">\n" +
                        "      <div class=\"item-icon\">🖥️</div>\n" +
                        "      <div class=\"item-title\">Raspberry Pi 5</div>\n" +
                        "      <div class=\"item-price\">$60.00</div>\n" +
                        "      <button class=\"btn-buy\" onclick=\"addToCart(60)\">Add to Cart</button>\n" +
                        "    </div>\n" +
                        "    <div class=\"item-card\">\n" +
                        "      <div class=\"item-icon\">⚡</div>\n" +
                        "      <div class=\"item-title\">Arduino Sensor Kit</div>\n" +
                        "      <div class=\"item-price\">$35.00</div>\n" +
                        "      <button class=\"btn-buy\" onclick=\"addToCart(35)\">Add to Cart</button>\n" +
                        "    </div>\n" +
                        "  </div>\n" +
                        "  <script>\n" +
                        "    var items = 0; var total = 0;\n" +
                        "    function addToCart(p) {\n" +
                        "      items++; total += p;\n" +
                        "      document.getElementById('cartItems').innerText = items;\n" +
                        "      document.getElementById('cartPrice').innerText = total.toFixed(2);\n" +
                        "    }\n" +
                        "  </script>\n" +
                        "</body>\n" +
                        "</html>\n";
                    break;

                default: // Interactive Click Counter App
                    var htmlSpec = LanguageRegistryService.GetLanguage("htmlcss");
                    TxtWebHtmlInput.Text = htmlSpec.StarterTemplateCode;
                    break;
            }

            RenderLiveWebsite();
        }

        private void BtnRenderWebsite_Click(object sender, RoutedEventArgs e)
        {
            RenderLiveWebsite();
        }

        private void RenderLiveWebsite()
        {
            try
            {
                if (WbLivePreview != null && TxtWebHtmlInput != null)
                {
                    SuppressScriptErrors(WbLivePreview);
                    string html = TxtWebHtmlInput.Text;
                    if (string.IsNullOrWhiteSpace(html))
                    {
                        html = "<!DOCTYPE html><html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"></head><body style='font-family:sans-serif; background:#0F172A; color:#fff; text-align:center; padding-top:40px;'><h2>Empty Web Document</h2><p>Write HTML in the left editor and click Render.</p></body></html>";
                    }
                    else if (!html.Contains("X-UA-Compatible") && html.Contains("<head>"))
                    {
                        html = html.Replace("<head>", "<head>\n  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
                    }
                    WbLivePreview.NavigateToString(html);
                    SuppressScriptErrors(WbLivePreview);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Render Error: {ex.Message}", "Web Previewer", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportHtml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string exportDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BhavaniTech_Projects");
                System.IO.Directory.CreateDirectory(exportDir);
                string filePath = System.IO.Path.Combine(exportDir, $"Website_Project_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                System.IO.File.WriteAllText(filePath, TxtWebHtmlInput.Text);
                MessageBox.Show($"Website exported successfully!\n\nFile Location:\n{filePath}\n\nYou can open this file in any browser (Chrome, Edge, Firefox) offline.", "Website Export", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export Error: {ex.Message}", "Website Export", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // =====================================================================
        // INTERACTIVE CALCULATOR PROJECT STUDIO
        // =====================================================================
        private void InitCalculatorStudio()
        {
            TxtCalcDisplay.Text = "0";
            TxtCalcExpression.Text = "";
            UpdateCalculatorSourceCode("JavaScript");
        }

        private void BtnCalcDigit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string digit)
            {
                if (_calcIsNewEntry)
                {
                    if (digit == ".")
                    {
                        _calcCurrentNumber = "0.";
                    }
                    else
                    {
                        _calcCurrentNumber = digit;
                    }
                    _calcIsNewEntry = false;
                }
                else
                {
                    if (digit == "." && _calcCurrentNumber.Contains("."))
                    {
                        return; // Ignore duplicate decimal
                    }
                    if (_calcCurrentNumber == "0" && digit != ".")
                    {
                        _calcCurrentNumber = digit;
                    }
                    else
                    {
                        _calcCurrentNumber += digit;
                    }
                }
                TxtCalcDisplay.Text = _calcCurrentNumber;
            }
        }

        private void BtnCalcOp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string op)
            {
                double currentVal = double.TryParse(_calcCurrentNumber, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val) ? val : 0.0;

                if (!string.IsNullOrEmpty(_calcPendingOp) && !_calcIsNewEntry)
                {
                    // Evaluate previous operation first
                    string expr = $"{_calcFirstOperand} {_calcPendingOp} {currentVal}";
                    var res = _calcEngine.Evaluate(expr);
                    if (res.Success)
                    {
                        _calcFirstOperand = res.Value;
                        _calcCurrentNumber = res.FormattedResult;
                        TxtCalcDisplay.Text = _calcCurrentNumber;
                    }
                }
                else
                {
                    _calcFirstOperand = currentVal;
                }

                _calcPendingOp = op;
                _calcIsNewEntry = true;
                string displayOp = op == "*" ? "×" : op == "/" ? "÷" : op == "-" ? "−" : "+";
                TxtCalcExpression.Text = $"{_calcFirstOperand} {displayOp}";
            }
        }

        private void BtnCalcEquals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_calcPendingOp)) return;

            double currentVal = double.TryParse(_calcCurrentNumber, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val) ? val : 0.0;
            string expr = $"{_calcFirstOperand} {_calcPendingOp} {currentVal}";
            var res = _calcEngine.Evaluate(expr);

            string displayOp = _calcPendingOp == "*" ? "×" : _calcPendingOp == "/" ? "÷" : _calcPendingOp == "-" ? "−" : "+";
            TxtCalcExpression.Text = $"{_calcFirstOperand} {displayOp} {currentVal} =";

            if (res.Success)
            {
                _calcCurrentNumber = res.FormattedResult;
                TxtCalcDisplay.Text = _calcCurrentNumber;
                _calcFirstOperand = res.Value;
            }
            else
            {
                TxtCalcDisplay.Text = "Error";
            }

            _calcPendingOp = "";
            _calcIsNewEntry = true;

            // Refresh history list
            LstCalcHistory.ItemsSource = _calcEngine.History.Select(h => $"{h.Expression} = {h.Result}").ToList();
        }

        private void BtnCalcClear_Click(object sender, RoutedEventArgs e)
        {
            _calcCurrentNumber = "0";
            _calcPendingOp = "";
            _calcFirstOperand = 0.0;
            _calcIsNewEntry = true;
            TxtCalcDisplay.Text = "0";
            TxtCalcExpression.Text = "";
        }

        private void BtnCalcClearEntry_Click(object sender, RoutedEventArgs e)
        {
            _calcCurrentNumber = "0";
            _calcIsNewEntry = true;
            TxtCalcDisplay.Text = "0";
        }

        private void BtnCalcBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (_calcIsNewEntry || _calcCurrentNumber.Length <= 1)
            {
                _calcCurrentNumber = "0";
                _calcIsNewEntry = true;
            }
            else
            {
                _calcCurrentNumber = _calcCurrentNumber.Substring(0, _calcCurrentNumber.Length - 1);
            }
            TxtCalcDisplay.Text = _calcCurrentNumber;
        }

        private void BtnCalcFunc_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string func)
            {
                double currentVal = double.TryParse(_calcCurrentNumber, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val) ? val : 0.0;
                double resVal = 0.0;
                string exprLabel = "";

                switch (func)
                {
                    case "sqr":
                        resVal = CalculatorEngine.CalculateSquare(currentVal);
                        exprLabel = $"sqr({currentVal})";
                        break;
                    case "sqrt":
                        resVal = CalculatorEngine.CalculateSquareRoot(currentVal);
                        exprLabel = $"√({currentVal})";
                        break;
                    case "recip":
                        resVal = CalculatorEngine.CalculateReciprocal(currentVal);
                        exprLabel = $"1/({currentVal})";
                        break;
                    case "negate":
                        resVal = -currentVal;
                        exprLabel = $"negate({currentVal})";
                        break;
                    case "percent":
                        resVal = currentVal / 100.0;
                        exprLabel = $"{currentVal}%";
                        break;
                }

                if (double.IsNaN(resVal) || double.IsInfinity(resVal))
                {
                    TxtCalcDisplay.Text = "Error";
                }
                else
                {
                    _calcCurrentNumber = resVal % 1 == 0 ? resVal.ToString("N0", System.Globalization.CultureInfo.InvariantCulture) : resVal.ToString("G10", System.Globalization.CultureInfo.InvariantCulture);
                    TxtCalcDisplay.Text = _calcCurrentNumber;
                    TxtCalcExpression.Text = exprLabel;
                }
                _calcIsNewEntry = true;
            }
        }

        private void BtnCalcMemory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string memOp)
            {
                double currentVal = double.TryParse(_calcCurrentNumber, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val) ? val : 0.0;

                switch (memOp)
                {
                    case "MC":
                        _calcEngine.MemoryClear();
                        break;
                    case "MR":
                        _calcCurrentNumber = _calcEngine.MemoryStore.ToString("G10", System.Globalization.CultureInfo.InvariantCulture);
                        TxtCalcDisplay.Text = _calcCurrentNumber;
                        _calcIsNewEntry = true;
                        break;
                    case "M+":
                        _calcEngine.MemoryAdd(currentVal);
                        _calcIsNewEntry = true;
                        break;
                    case "M-":
                        _calcEngine.MemorySubtract(currentVal);
                        _calcIsNewEntry = true;
                        break;
                }
            }
        }

        private void CmbCalcCodeLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCalcCodeLang?.SelectedItem is ComboBoxItem item)
            {
                string lang = item.Content.ToString() ?? "JavaScript";
                UpdateCalculatorSourceCode(lang);
            }
        }

        private void UpdateCalculatorSourceCode(string lang)
        {
            if (TxtCalcSourceCode == null) return;

            if (lang.Contains("JavaScript"))
            {
                TxtCalcSourceCode.Text = 
                    "// ========================================================\n" +
                    "// Bhavani Tech — Complete Working Web Calculator (JavaScript)\n" +
                    "// ========================================================\n\n" +
                    "class Calculator {\n" +
                    "    constructor(displayElement, expressionElement) {\n" +
                    "        this.display = displayElement;\n" +
                    "        this.expression = expressionElement;\n" +
                    "        this.currentValue = '0';\n" +
                    "        this.firstOperand = null;\n" +
                    "        this.operator = null;\n" +
                    "        this.waitingForSecondOperand = false;\n" +
                    "    }\n\n" +
                    "    inputDigit(digit) {\n" +
                    "        if (this.waitingForSecondOperand) {\n" +
                    "            this.currentValue = digit;\n" +
                    "            this.waitingForSecondOperand = false;\n" +
                    "        } else {\n" +
                    "            this.currentValue = this.currentValue === '0' ? digit : this.currentValue + digit;\n" +
                    "        }\n" +
                    "        this.updateDisplay();\n" +
                    "    }\n\n" +
                    "    handleOperator(nextOperator) {\n" +
                    "        const inputValue = parseFloat(this.currentValue);\n" +
                    "        if (this.operator && this.waitingForSecondOperand) {\n" +
                    "            this.operator = nextOperator;\n" +
                    "            this.expression.innerText = `${this.firstOperand} ${nextOperator}`;\n" +
                    "            return;\n" +
                    "        }\n\n" +
                    "        if (this.firstOperand == null && !isNaN(inputValue)) {\n" +
                    "            this.firstOperand = inputValue;\n" +
                    "        } else if (this.operator) {\n" +
                    "            const result = this.calculate(this.firstOperand, inputValue, this.operator);\n" +
                    "            this.currentValue = `${parseFloat(result.toFixed(7))}`;\n" +
                    "            this.firstOperand = result;\n" +
                    "        }\n\n" +
                    "        this.waitingForSecondOperand = true;\n" +
                    "        this.operator = nextOperator;\n" +
                    "        this.expression.innerText = `${this.firstOperand} ${nextOperator}`;\n" +
                    "        this.updateDisplay();\n" +
                    "    }\n\n" +
                    "    calculate(first, second, op) {\n" +
                    "        switch (op) {\n" +
                    "            case '+': return first + second;\n" +
                    "            case '-': return first - second;\n" +
                    "            case '*': return first * second;\n" +
                    "            case '/': return second !== 0 ? first / second : 'Error';\n" +
                    "            default: return second;\n" +
                    "        }\n" +
                    "    }\n\n" +
                    "    computeEquals() {\n" +
                    "        if (this.operator == null || this.waitingForSecondOperand) return;\n" +
                    "        const inputValue = parseFloat(this.currentValue);\n" +
                    "        const result = this.calculate(this.firstOperand, inputValue, this.operator);\n" +
                    "        this.expression.innerText = `${this.firstOperand} ${this.operator} ${inputValue} =`;\n" +
                    "        this.currentValue = `${parseFloat(result.toFixed(7))}`;\n" +
                    "        this.firstOperand = null;\n" +
                    "        this.operator = null;\n" +
                    "        this.waitingForSecondOperand = false;\n" +
                    "        this.updateDisplay();\n" +
                    "    }\n\n" +
                    "    updateDisplay() {\n" +
                    "        this.display.innerText = this.currentValue;\n" +
                    "    }\n" +
                    "}";
            }
            else if (lang.Contains("C#"))
            {
                TxtCalcSourceCode.Text = 
                    "// ========================================================\n" +
                    "// Bhavani Tech — C# .NET 9 WPF Calculator Logic Engine\n" +
                    "// ========================================================\n" +
                    "using System;\n" +
                    "using System.Data;\n\n" +
                    "public class WpfCalculatorEngine\n" +
                    "{\n" +
                    "    private string _currentDisplay = \"0\";\n" +
                    "    private string _expression = \"\";\n\n" +
                    "    public string Display => _currentDisplay;\n" +
                    "    public string Expression => _expression;\n\n" +
                    "    public void PressDigit(string digit)\n" +
                    "    {\n" +
                    "        if (_currentDisplay == \"0\" && digit != \".\")\n" +
                    "            _currentDisplay = digit;\n" +
                    "        else\n" +
                    "            _currentDisplay += digit;\n" +
                    "    }\n\n" +
                    "    public void PressOperator(string op)\n" +
                    "    {\n" +
                    "        _expression = $\"{_currentDisplay} {op}\";\n" +
                    "        _currentDisplay = \"0\";\n" +
                    "    }\n\n" +
                    "    public void Evaluate()\n" +
                    "    {\n" +
                    "        string fullExpr = $\"{_expression} {_currentDisplay}\";\n" +
                    "        var dt = new DataTable();\n" +
                    "        var result = dt.Compute(fullExpr, null);\n" +
                    "        _currentDisplay = Convert.ToDouble(result).ToString(\"G10\");\n" +
                    "        _expression = $\"{fullExpr} =\";\n" +
                    "    }\n" +
                    "}";
            }
            else
            {
                TxtCalcSourceCode.Text = 
                    "# ========================================================\n" +
                    "# Bhavani Tech — Python Tkinter GUI Calculator Engine\n" +
                    "# ========================================================\n" +
                    "import tkinter as tk\n\n" +
                    "class CalculatorApp:\n" +
                    "    def __init__(self, root):\n" +
                    "        self.root = root\n" +
                    "        self.root.title('Bhavani Python Calculator')\n" +
                    "        self.expr = ''\n" +
                    "        \n" +
                    "        self.display = tk.Entry(root, font=('Consolas', 20), justify='right', bd=10)\n" +
                    "        self.display.grid(row=0, column=0, columnspan=4, padx=10, pady=10)\n" +
                    "        \n" +
                    "        buttons = [\n" +
                    "            ('7', 1, 0), ('8', 1, 1), ('9', 1, 2), ('/', 1, 3),\n" +
                    "            ('4', 2, 0), ('5', 2, 1), ('6', 2, 2), ('*', 2, 3),\n" +
                    "            ('1', 3, 0), ('2', 3, 1), ('3', 3, 2), ('-', 3, 3),\n" +
                    "            ('C', 4, 0), ('0', 4, 1), ('=', 4, 2), ('+', 4, 3),\n" +
                    "        ]\n" +
                    "        for text, r, c in buttons:\n" +
                    "            cmd = lambda t=text: self.on_click(t)\n" +
                    "            tk.Button(root, text=text, width=5, height=2, font=('Segoe UI', 14), command=cmd).grid(row=r, column=c, padx=4, pady=4)\n" +
                    "            \n" +
                    "    def on_click(self, char):\n" +
                    "        if char == '=':\n" +
                    "            try:\n" +
                    "                res = str(eval(self.expr))\n" +
                    "                self.display.delete(0, tk.END)\n" +
                    "                self.display.insert(0, res)\n" +
                    "                self.expr = res\n" +
                    "            except Exception:\n" +
                    "                self.display.delete(0, tk.END)\n" +
                    "                self.display.insert(0, 'Error')\n" +
                    "        elif char == 'C':\n" +
                    "            self.expr = ''\n" +
                    "            self.display.delete(0, tk.END)\n" +
                    "        else:\n" +
                    "            self.expr += str(char)\n" +
                    "            self.display.delete(0, tk.END)\n" +
                    "            self.display.insert(0, self.expr)\n";
            }
        }

        private void BtnCopyCalcCode_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCalcSourceCode != null && !string.IsNullOrWhiteSpace(TxtCalcSourceCode.Text))
            {
                Clipboard.SetText(TxtCalcSourceCode.Text);
                MessageBox.Show("Calculator Source Code copied to Clipboard!\nYou can paste it in the Code IDE or your external editor.", "Calculator Code", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        // SOFTWARE ENGINEERING LAB
        private void BtnSimulateScheduler_Click(object sender, RoutedEventArgs e)
        {
            string algo = (CmbProcessAlgo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "FIFO";
            if (algo.Contains("SJF")) algo = "SJF";
            else if (algo.Contains("Priority")) algo = "Priority";
            else algo = "FIFO";

            var procs = new List<ProcessItem>
            {
                new ProcessItem("P1_Browser_Task", 15, 2),
                new ProcessItem("P2_Code_Runner", 5, 1),
                new ProcessItem("P3_Audio_Service", 8, 3),
                new ProcessItem("P4_Database_Write", 12, 1)
            };

            var res = SoftwareSimulationService.SimulateProcessScheduling(algo, procs);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== OS CPU PROCESS SCHEDULER SIMULATION ({res.Algorithm}) ===");
            sb.AppendLine($"Average Wait Time        : {res.AverageWaitTimeMs} ms");
            sb.AppendLine($"Average Turnaround Time  : {res.AverageTurnaroundTimeMs} ms");
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine("EXECUTION TIMELINE:");
            foreach (var s in res.ExecutionSequence) sb.AppendLine($" - {s}");

            TxtSchedulerOutput.Text = sb.ToString();
        }

        private void BtnRunLexer_Click(object sender, RoutedEventArgs e)
        {
            var res = SoftwareSimulationService.RunCompilerLexer(TxtLexerInput.Text);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== COMPILER LEXICAL ANALYSIS ({res.TotalTokens} TOKENS) ===");
            sb.AppendLine("TOKEN TYPE\tLITERAL\tLINE");
            sb.AppendLine("---------------------------------------------------------");
            foreach (var t in res.Tokens)
            {
                sb.AppendLine($"{t.TokenType}\t{t.Literal}\tLine {t.LineNumber}");
            }
            TxtLexerOutput.Text = sb.ToString();
        }

        private void BtnViewGitLog_Click(object sender, RoutedEventArgs e)
        {
            var commits = SoftwareSimulationService.RunSimulatedGitLog();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== GIT VERSION CONTROL COMMIT GRAPH ===");
            foreach (var c in commits)
            {
                sb.AppendLine($"* commit {c.CommitHash}");
                sb.AppendLine($"| Author: {c.Author}");
                sb.AppendLine($"| Date:   {c.Timestamp:yyyy-MM-dd HH:mm}");
                sb.AppendLine($"|     {c.Message}\n|");
            }
            TxtGitLogOutput.Text = sb.ToString();
        }

        // VIRTUAL PC BUILDER & BUS CALCULATORS
        private void BtnValidateBuild_Click(object sender, RoutedEventArgs e)
        {
            string cpuSel = (CmbCpu.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string mbSel = (CmbMotherboard.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string gpuSel = (CmbGpu.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string psuSel = (CmbPsu.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";

            var cpu = new PcComponent("CPU", cpuSel, cpuSel.Contains("12100") ? "LGA1700" : "AM4", 65, 120, "");
            var mb = new PcComponent("Motherboard", mbSel, mbSel.Contains("B660M") ? "LGA1700" : "AM4", 20, 100, "");
            var gpu = new PcComponent("GPU", gpuSel, "PCIe", gpuSel.Contains("3060") ? 170 : (gpuSel.Contains("1650") ? 75 : 15), 250, "");
            var psu = new PcComponent("PSU", psuSel, "", psuSel.Contains("500W") ? 500 : 300, 50, "");

            var val = HardwareSimulationService.ValidatePcBuild(cpu, mb, new PcComponent("RAM", "16GB DDR4", "", 5, 40, ""), gpu, psu);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"COMPATIBILITY STATUS: {(val.IsCompatible ? "PASSED ✅" : "FAILED ❌")}");
            sb.AppendLine($"Estimated Total Power Draw: {val.EstimatedPowerUsageWatts} Watts");
            sb.AppendLine("---------------------------------------------------------");
            if (val.Errors.Count > 0)
            {
                sb.AppendLine("ERRORS:");
                foreach (var err in val.Errors) sb.AppendLine($" - ❌ {err}");
            }
            if (val.Warnings.Count > 0)
            {
                sb.AppendLine("WARNINGS:");
                foreach (var warn in val.Warnings) sb.AppendLine($" - ⚠️ {warn}");
            }
            if (val.IsCompatible && val.Warnings.Count == 0)
            {
                sb.AppendLine("All components are 100% compatible! System is ready to assemble.");
            }

            TxtHardwareOutput.Text = sb.ToString();
        }

        private void BtnCalcMemorySpeed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double speed = double.Parse(TxtRamSpeed.Text.Trim());
                int cl = int.Parse(TxtRamCl.Text.Trim());

                var memRes = HardwareSimulationService.CalculateMemoryPerformance(speed, cl, true);
                var pcieRes = HardwareSimulationService.CalculatePcieBandwidth(4, 16);

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("=== RAM MEMORY & PCIE BUS PERFORMANCE ===");
                sb.AppendLine($"RAM Clock Transfer Speed : {memRes.TransferRateMTS} MT/s (MHz)");
                sb.AppendLine($"True First Word Latency  : {memRes.LatencyNanoseconds} ns");
                sb.AppendLine($"Dual-Channel Bandwidth   : {memRes.DualChannelBandwidthGBps} GB/s");
                sb.AppendLine("---------------------------------------------------------");
                sb.AppendLine($"PCIe Expansion Bus Speed : {pcieRes.PcieGen} x16");
                sb.AppendLine($"Max PCIe Data Bandwidth  : {pcieRes.MaxBandwidthGBps} GB/s");

                TxtMemorySpeedOutput.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                TxtMemorySpeedOutput.Text = $"Calculation Error: {ex.Message}";
            }
        }

        // NETWORKING
        private void BtnSimulatePing_Click(object sender, RoutedEventArgs e)
        {
            var steps = NetworkSimulationService.SimulatePing("192.168.1.10", "8.8.8.8");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== ICMP PING PACKET SIMULATION ===");
            foreach (var step in steps)
            {
                sb.AppendLine($"[{step.StepDescription}] {step.SourceDevice} -> {step.DestinationDevice}");
                sb.AppendLine($"   Info: {step.LayerInfo} | Details: {step.HeaderDetails}\n");
            }
            TxtNetworkPacketOutput.Text = sb.ToString();
        }

        private void BtnSimulateDhcp_Click(object sender, RoutedEventArgs e)
        {
            var steps = NetworkSimulationService.SimulateDhcpDns("bhavanitech.edu");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== DHCP & DNS RESOLUTION SIMULATION ===");
            foreach (var step in steps)
            {
                sb.AppendLine($"[{step.StepDescription}] {step.SourceDevice} -> {step.DestinationDevice}");
                sb.AppendLine($"   Info: {step.LayerInfo} | Details: {step.HeaderDetails}\n");
            }
            TxtNetworkPacketOutput.Text = sb.ToString();
        }

        private void BtnCalcSubnet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = TxtSubnetIp.Text.Trim();
                int cidr = int.Parse(TxtSubnetCidr.Text.Trim());
                var res = SubnettingEngine.CalculateSubnet(ip, cidr);

                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"SUBNET CALCULATION RESULT FOR {ip}/{cidr}");
                sb.AppendLine("---------------------------------------------------------");
                sb.AppendLine($"Network Address  : {res.NetworkAddress}");
                sb.AppendLine($"Broadcast Address: {res.BroadcastAddress}");
                sb.AppendLine($"Subnet Netmask   : {res.Netmask}");
                sb.AppendLine($"Wildcard Mask    : {res.WildcardMask}");
                sb.AppendLine($"First Usable IP  : {res.FirstUsableIP}");
                sb.AppendLine($"Last Usable IP   : {res.LastUsableIP}");
                sb.AppendLine($"Total Usable Hosts: {res.UsableHosts:N0}");
                TxtSubnetResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                TxtSubnetResult.Text = $"Subnet Error: {ex.Message}";
            }
        }

        // =====================================================================
        // CYBER ETHICAL HACKING WITH LOCAL AI & ADVANCED LABS
        // =====================================================================
        private void BtnCyberChip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string prompt)
            {
                TxtCyberAiPrompt.Text = prompt;
                BtnAskCyberAi_Click(sender, e);
            }
        }

        private void BtnAskCyberAi_Click(object sender, RoutedEventArgs e)
        {
            string prompt = TxtCyberAiPrompt.Text.Trim();
            if (string.IsNullOrEmpty(prompt)) return;

            var res = LocalAiEngine.QueryLocalAi(prompt, "Cybersecurity");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== 🤖 CYBER ETHICAL HACKING LOCAL AI MENTOR ===");
            sb.AppendLine($"TOPIC: {res.Topic} [{res.MasteryLevel}] | CONFIDENCE: {res.ConfidenceScore * 100:F0}%");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine(res.AnswerText);
            if (!string.IsNullOrEmpty(res.CodeExample))
            {
                sb.AppendLine("\nEXPLOIT / DEFENSE CODE TEMPLATE:");
                sb.AppendLine(res.CodeExample);
            }
            if (res.RecommendedFollowUps.Count > 0)
            {
                sb.AppendLine("\nRECOMMENDED SECURITY FOLLOW-UPS:");
                foreach (var f in res.RecommendedFollowUps) sb.AppendLine($" - 🛡️ {f}");
            }

            TxtCyberAiResponse.Text = sb.ToString();
        }

        // CTF ARENA
        private void LoadCtfChallenges()
        {
            if (CmbCtfChallenge == null) return;
            _ctfChallenges = CybersecurityLabService.GetCtfChallenges();
            CmbCtfChallenge.ItemsSource = _ctfChallenges.Select(c => $"#{c.Id} [{c.Category} - {c.Difficulty}] {c.Title}").ToList();
            if (_ctfChallenges.Count > 0)
            {
                CmbCtfChallenge.SelectedIndex = 0;
            }
        }

        private void CmbCtfChallenge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCtfChallenge == null || TxtCtfMission == null) return;
            int idx = CmbCtfChallenge.SelectedIndex;
            if (idx >= 0 && idx < _ctfChallenges.Count)
            {
                var ch = _ctfChallenges[idx];
                TxtCtfMission.Text = $"{ch.MissionBriefing}\n\nReward: ⭐ {ch.XpReward} XP";
                TxtCtfFlagInput.Text = "FLAG{...}";
                TxtCtfResult.Text = "Read the mission briefing above, uncover the secret flag, and submit it here!";
            }
        }

        private void BtnCtfHint_Click(object sender, RoutedEventArgs e)
        {
            int idx = CmbCtfChallenge.SelectedIndex;
            if (idx >= 0 && idx < _ctfChallenges.Count)
            {
                var ch = _ctfChallenges[idx];
                TxtCtfResult.Text = $"💡 HINT FOR {ch.Title}:\n{ch.Hint}";
            }
        }

        private void BtnSubmitCtfFlag_Click(object sender, RoutedEventArgs e)
        {
            int idx = CmbCtfChallenge.SelectedIndex;
            if (idx >= 0 && idx < _ctfChallenges.Count)
            {
                var ch = _ctfChallenges[idx];
                string flag = TxtCtfFlagInput.Text.Trim();
                var val = CybersecurityLabService.ValidateCtfFlag(ch.Id, flag);

                if (val.Success)
                {
                    int totalXp = _db.AddUserXp(val.XpEarned);
                    TxtUserXp.Text = $"⭐ {totalXp} XP";
                    TxtCtfResult.Foreground = (Brush)new BrushConverter().ConvertFrom("#4ADE80")!;
                    TxtCtfResult.Text = $"🎉 {val.FeedbackMessage}\n\nChallenge Solved! You've successfully completed this mission.";
                }
                else
                {
                    TxtCtfResult.Foreground = (Brush)new BrushConverter().ConvertFrom("#F87171")!;
                    TxtCtfResult.Text = $"❌ {val.FeedbackMessage}";
                }
            }
        }

        // PACKET SNIFFER
        private void BtnInspectPackets_Click(object sender, RoutedEventArgs e)
        {
            var packets = CybersecurityLabService.GetSimulatedPacketStream();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== WIRESHARK-STYLE LIVE PACKET STREAM CAPTURE ===");
            sb.AppendLine("NO.\tTIME\tSRC IP\t\tDST IP\t\tPROTO\tLEN\tINFO");
            sb.AppendLine("----------------------------------------------------------------------------------------------------");

            foreach (var p in packets)
            {
                string alert = p.IsPlaintextVulnerable ? " ⚠️ [PLAINTEXT CREDENTIALS LEAKED!]" : "";
                sb.AppendLine($"{p.PacketNumber}\t{p.Timestamp}\t{p.SourceIp}\t{p.DestinationIp}\t{p.Protocol}\t{p.Length}\t{p.Summary}{alert}");
                sb.AppendLine($"   -> Payload Preview: {p.RawPayloadText}\n");
            }

            TxtPacketStreamOutput.Text = sb.ToString();
        }

        // DICTIONARY ATTACK
        private void BtnRunDictAttack_Click(object sender, RoutedEventArgs e)
        {
            string targetHash = TxtDictTargetHash.Text.Trim();
            bool isMd5 = RbDictMd5.IsChecked == true;

            var res = CybersecurityLabService.RunDictionaryAttack(targetHash, isMd5);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== OFFLINE DICTIONARY ATTACK CRACKER RESULT ===");
            sb.AppendLine($"Hash Algorithm Tested : {(isMd5 ? "MD5 (Insecure)" : "SHA-256")}");
            sb.AppendLine($"Target Hash           : {targetHash}");
            sb.AppendLine($"Dictionary Attempts   : {res.AttemptsMade} words");
            sb.AppendLine($"Elapsed Time          : {res.ElapsedTimeMs:F2} ms");
            sb.AppendLine($"Outcome               : {(res.Cracked ? $"PLAINTEXT FOUND: '{res.PlaintextFound}'" : "NOT CRACKED")}");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine($"SECURITY ANALYSIS:\n{res.SecurityAnalysis}");

            TxtDictCrackOutput.Text = sb.ToString();
        }

        // CYBERSECURITY LAB SCANS
        private void BtnScanPorts_Click(object sender, RoutedEventArgs e)
        {
            var ports = CybersecurityLabService.RunSimulatedPortScan("127.0.0.1");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== LOCAL SIMULATED PORT SCAN RESULTS (127.0.0.1) ===");
            sb.AppendLine("PORT\tSERVICE\tSTATUS\tRISK ASSESSMENT");
            sb.AppendLine("---------------------------------------------------------");
            foreach (var p in ports)
            {
                sb.AppendLine($"{p.Port}\t{p.Service}\t{p.Status}\t{p.RiskLevel}");
            }
            TxtPortScanOutput.Text = sb.ToString();
        }

        private void BtnTestSqliUnsafe_Click(object sender, RoutedEventArgs e)
        {
            var res = CybersecurityLabService.TestSqlInjectionLab(TxtSqliUser.Text, TxtSqliPass.Text, false);
            TxtSqliResult.Text = $"EXECUTED QUERY:\n{res.QueryExecuted}\n\nOUTCOME:\n{res.ResultData}\n\nADVICE:\n{res.SecurityAdvice}";
        }

        private void BtnTestSqliSafe_Click(object sender, RoutedEventArgs e)
        {
            var res = CybersecurityLabService.TestSqlInjectionLab(TxtSqliUser.Text, TxtSqliPass.Text, true);
            TxtSqliResult.Text = $"EXECUTED QUERY:\n{res.QueryExecuted}\n\nOUTCOME:\n{res.ResultData}\n\nADVICE:\n{res.SecurityAdvice}";
        }

        private void BtnTestXssUnsafe_Click(object sender, RoutedEventArgs e)
        {
            var res = CybersecurityLabService.TestXssLab(TxtXssInput.Text, false);
            TxtXssResult.Text = $"RAW INPUT PAYLOAD:\n{res.RawInput}\n\nRENDERED OUTPUT:\n{res.RenderedOutput}\n\nSTATUS:\n{(res.ScriptExecuted ? "VULNERABLE (Script Executed!)" : "Safe")}\n\nSECURITY ADVICE:\n{res.SecurityAdvice}";
        }

        private void BtnTestXssSafe_Click(object sender, RoutedEventArgs e)
        {
            var res = CybersecurityLabService.TestXssLab(TxtXssInput.Text, true);
            TxtXssResult.Text = $"RAW INPUT PAYLOAD:\n{res.RawInput}\n\nHTML ENTITY ENCODED OUTPUT:\n{res.RenderedOutput}\n\nSTATUS:\nPROTECTED (Entities Escaped!)\n\nSECURITY ADVICE:\n{res.SecurityAdvice}";
        }

        private void TxtHashInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtHashInput == null || TxtHashOutput == null) return;

            string raw = TxtHashInput.Text;
            string sha = CybersecurityLabService.ComputeSha256(raw);
            string base64 = CybersecurityLabService.Base64Encode(raw);
            var eval = CybersecurityLabService.EvaluatePassword(raw);

            TxtHashOutput.Text = $"SHA-256 HASH:\n{sha}\n\nBASE64 ENCODING:\n{base64}\n\nPASSWORD ENTROPY:\nEntropy: {eval.EntropyBits} bits | Rating: {eval.Rating}\nAdvice: {eval.Feedback}";
        }

        private void BtnScanLogForensics_Click(object sender, RoutedEventArgs e)
        {
            string mockLog = @"2026-09-05 08:00:01 INFO [Auth] User 'student' logged in successfully from 192.168.1.10
2026-09-05 08:05:12 WARN [Auth] FAILED_LOGIN attempt for user 'admin' from 203.0.113.45
2026-09-05 08:05:14 WARN [Auth] FAILED_LOGIN attempt for user 'admin' from 203.0.113.45
2026-09-05 08:05:16 WARN [Auth] FAILED_LOGIN attempt for user 'root' from 203.0.113.45
2026-09-05 08:05:18 WARN [Auth] FAILED_LOGIN attempt for user 'administrator' from 203.0.113.45
2026-09-05 08:10:00 INFO [System] Automated backup completed.";

            var res = CybersecurityLabService.AnalyzeLogFile(mockLog);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== DIGITAL FORENSICS LOG ANALYZER REPORT ===");
            sb.AppendLine($"Total Log Entries Analyzed : {res.TotalLinesScanned}");
            sb.AppendLine($"Failed Authentication Events: {res.FailedLoginAttempts}");
            sb.AppendLine($"Suspicious Remote IPs       : {res.SuspiciousIPsCount}");
            sb.AppendLine("---------------------------------------------------------");
            foreach (var alert in res.SecurityAlerts)
            {
                sb.AppendLine(alert);
            }
            TxtLogForensicsOutput.Text = sb.ToString();
        }

        // AI ACADEMY
        private void BtnAnalyzePrompt_Click(object sender, RoutedEventArgs e)
        {
            var res = AiAcademyService.AnalyzePrompt(TxtPromptInput.Text);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"PROMPT PRECISION RATING: {res.Rating}");
            sb.AppendLine($"Estimated Token Overhead: ~{res.EstimatedTokens} tokens");
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine("FEEDBACK & CRITIQUE:");
            sb.AppendLine(res.Feedback);
            sb.AppendLine("OPTIMIZED PROMPT TEMPLATE:");
            sb.AppendLine(res.OptimizedPrompt);
            TxtPromptResult.Text = sb.ToString();
        }

        private void BtnSendAiQuestion_Click(object sender, RoutedEventArgs e)
        {
            string q = TxtAiQuestion.Text.Trim();
            if (string.IsNullOrEmpty(q)) return;

            string domain = (CmbAiDomain?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "All Domains";
            if (domain == "All Domains") domain = "All";

            var res = LocalAiEngine.QueryLocalAi(q, domain);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"\nStudent: {q}");
            sb.AppendLine($"AI Tutor [{res.Topic} ({res.Domain} - {res.MasteryLevel}) - Confidence: {res.ConfidenceScore * 100:F0}%]:");
            sb.AppendLine(res.AnswerText);
            if (!string.IsNullOrEmpty(res.CodeExample))
            {
                sb.AppendLine("\nCODE EXAMPLE:");
                sb.AppendLine(res.CodeExample);
            }
            if (res.RecommendedFollowUps.Count > 0)
            {
                sb.AppendLine("\nRECOMMENDED NEXT LESSONS:");
                foreach (var f in res.RecommendedFollowUps) sb.AppendLine($" - 💡 {f}");
            }

            TxtAiChatHistory.AppendText(sb.ToString() + "\n");
            TxtAiChatHistory.ScrollToEnd();
            TxtAiQuestion.Clear();
        }

        // ELECTRONICS ACADEMY
        private void BtnCalcOhmsLaw_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double? v = double.TryParse(TxtOhmsVoltage.Text, out double vVal) ? vVal : null;
                double? i = double.TryParse(TxtOhmsCurrent.Text, out double iVal) ? iVal : null;
                double? r = double.TryParse(TxtOhmsResistance.Text, out double rVal) ? rVal : null;

                var res = ElectronicsSimulationService.CalculateOhmsLaw(v, i, r);
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("=== OHM'S LAW & CIRCUIT CALCULATION ===");
                sb.AppendLine($"Voltage (V)    : {res.Voltage} Volts");
                sb.AppendLine($"Current (I)    : {res.CurrentAmps} Amperes ({res.CurrentAmps * 1000:F1} mA)");
                sb.AppendLine($"Resistance (R) : {res.ResistanceOhms} Ohms (Ω)");
                sb.AppendLine($"Power (P)      : {res.PowerWatts} Watts");
                
                // LED calculation
                var ledRes = ElectronicsSimulationService.CalculateLedResistor(res.Voltage, 2.0, 0.02);
                sb.AppendLine($"\nLED Current Limiter Resistor (for 2V LED @ 20mA):");
                sb.AppendLine($" - Calculated Resistance : {ledRes.RequiredResistorOhms} Ω");
                sb.AppendLine($" - Standard E24 Resistor : {ledRes.RecommendedStandardResistorOhms} Ω");

                TxtOhmsResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                TxtOhmsResult.Text = $"Calculation Error: {ex.Message}";
            }
        }

        private void BtnEvaluateLogicGate_Click(object sender, RoutedEventArgs e)
        {
            string gate = (CmbLogicGate.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "AND";
            bool inA = (CmbLogicInA.SelectedItem as ComboBoxItem)?.Content.ToString()?.StartsWith("1") == true;
            bool inB = (CmbLogicInB.SelectedItem as ComboBoxItem)?.Content.ToString()?.StartsWith("1") == true;

            bool output = ElectronicsSimulationService.EvaluateLogicGate(gate, inA, inB);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== LOGIC GATE EVALUATION: {gate} ===");
            sb.AppendLine($"Input A : {(inA ? "1 (HIGH)" : "0 (LOW)")}");
            sb.AppendLine($"Input B : {(inB ? "1 (HIGH)" : "0 (LOW)")}");
            sb.AppendLine($"OUTPUT  : {(output ? "1 (HIGH - TRUE)" : "0 (LOW - FALSE)")}");
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine($"Gate Truth Rule: {gate} gate outputs HIGH under boolean condition.");

            TxtLogicGateResult.Text = sb.ToString();
        }

        // LINUX TERMINAL
        private void InitLinuxTerminal()
        {
            if (string.IsNullOrEmpty(TxtLinuxTerminalOutput.Text))
            {
                TxtLinuxTerminalOutput.Text = "Bhavani Technology Linux Lab [Kernel 6.1.0-low-spec]\nType 'help' to see list of available simulated Linux commands.\n";
            }
        }

        private void TxtLinuxCommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string cmd = TxtLinuxCommandInput.Text.Trim();
                TxtLinuxCommandInput.Clear();
                ExecuteLinuxCommand(cmd);
            }
        }

        private void ExecuteLinuxCommand(string cmd)
        {
            var res = _terminalSession.Execute(cmd);
            if (res.Output == "__CLEAR__")
            {
                TxtLinuxTerminalOutput.Clear();
            }
            else
            {
                string prompt = $"{_terminalSession.CurrentUser}@bhavani-box:{_terminalSession.CurrentPath}$ ";
                TxtLinuxTerminalOutput.AppendText($"{prompt}{cmd}\n");
                if (!string.IsNullOrEmpty(res.Output))
                {
                    TxtLinuxTerminalOutput.AppendText($"{res.Output}\n");
                }
                if (res.FlagDiscovered && !string.IsNullOrEmpty(res.FlagValue))
                {
                    TxtLinuxTerminalOutput.AppendText($"\n🎉 CTF MISSION ACCOMPLISHED! Flag: {res.FlagValue} (+{res.XpEarned} XP Awarded!)\n\n");
                    int totalXp = _db.AddUserXp(res.XpEarned);
                    TxtUserXp.Text = $"⭐ {totalXp} XP";
                }
                TxtLinuxTerminalOutput.ScrollToEnd();
            }

            TxtLinuxPrompt.Text = $"{_terminalSession.CurrentUser}@bhavani-box:{_terminalSession.CurrentPath}$ ";
        }

        // IT TROUBLESHOOTING (500+ SCENARIOS)
        private void LoadTroubleshootingScenarios()
        {
            if (LstTroubleScenarios == null) return;

            string cat = (CmbTroubleCategory?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "All Categories";
            string kw = (TxtTroubleSearch != null && TxtTroubleSearch.Text != "Search 500+ scenarios...") ? TxtTroubleSearch.Text.Trim() : "";

            _activeScenarios = _db.GetTroubleshootingScenarios(cat, kw);
            LstTroubleScenarios.ItemsSource = _activeScenarios.Select(s => $"#{s.Id} [{s.Category}] {s.Title}").ToList();

            if (_activeScenarios.Count > 0)
            {
                LstTroubleScenarios.SelectedIndex = 0;
            }
            else
            {
                TxtTroubleTitle.Text = "No Scenarios Found";
                TxtTroubleMeta.Text = "Category: -- | Difficulty: --";
                TxtTroubleLog.Text = "No troubleshooting scenarios matched your search filter.";
            }
        }

        private void CmbTroubleCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTroubleshootingScenarios();
        }

        private void TxtTroubleSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtTroubleSearch.Text == "Search 500+ scenarios...")
            {
                TxtTroubleSearch.Text = "";
            }
        }

        private void TxtTroubleSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtTroubleSearch == null || LstTroubleScenarios == null) return;
            LoadTroubleshootingScenarios();
        }

        private void LstTroubleScenarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstTroubleScenarios == null || TxtTroubleTitle == null || TxtTroubleLog == null) return;

            int idx = LstTroubleScenarios.SelectedIndex;
            if (idx >= 0 && idx < _activeScenarios.Count)
            {
                var s = _activeScenarios[idx];
                TxtTroubleTitle.Text = s.Title;
                TxtTroubleMeta.Text = $"Category: {s.Category} | Difficulty: {s.Difficulty}";

                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"SCENARIO #{s.Id}: {s.Title}");
                sb.AppendLine("---------------------------------------------------------");
                sb.AppendLine($"SYMPTOMS:\n{s.Symptoms}\n");
                sb.AppendLine($"ROOT CAUSE ANALYSIS:\n{s.RootCause}\n");
                sb.AppendLine($"STEP-BY-STEP DIAGNOSTIC & RESOLUTION PROCEDURE:\n{s.ResolutionSteps}");
                TxtTroubleLog.Text = sb.ToString();
            }
        }

        // PERFORMANCE SETTINGS
        private void ChkSoftwareRender_Click(object sender, RoutedEventArgs e)
        {
            if (ChkSoftwareRender.IsChecked == true)
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
                TxtPerfStatus.Text = "Mode: LOW-HARDWARE (GDI+)";
            }
            else
            {
                RenderOptions.ProcessRenderMode = RenderMode.Default;
                TxtPerfStatus.Text = "Mode: NORMAL (HARDWARE)";
            }
        }

        // =====================================================================
        // DEEP-TECH SIMULATOR HANDLERS (CONTAINERS, ROP, ATTENTION, QUANTUM)
        // =====================================================================
        private void BtnSimulateContainer_Click(object sender, RoutedEventArgs e)
        {
            string name = string.IsNullOrWhiteSpace(TxtContainerName.Text) ? "bhavani-sandbox-v1" : TxtContainerName.Text.Trim();
            int.TryParse(TxtContainerCpuQuota.Text, out int cpu);
            int.TryParse(TxtContainerMemoryLimit.Text, out int mem);

            var res = SoftwareSimulationService.SimulateContainerNamespaces(name, cpu, mem);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(res.KernelExplanation);
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine($"Container Instance  : {res.ContainerName}");
            sb.AppendLine($"Host Process ID     : {res.HostPid} (External System PID)");
            sb.AppendLine($"Container PID Tree  : PID {res.ContainerPid} (Namespaced /init)");
            sb.AppendLine($"Rootfs Mount Point  : {res.RootFsMount}");
            sb.AppendLine($"Virtual veth Pair   : {res.VirtualNetInterface} -> {res.AssignedIp}");
            sb.AppendLine($"cgroups v2 cpu.max  : {res.CgroupsCpuLimit}");
            sb.AppendLine($"cgroups v2 memory   : {res.CgroupsMemoryLimit}");
            TxtContainerOutput.Text = sb.ToString();

            int totalXp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
        }

        private void BtnSimulateRop_Click(object sender, RoutedEventArgs e)
        {
            string target = string.IsNullOrWhiteSpace(TxtRopTargetCommand.Text) ? "/bin/sh" : TxtRopTargetCommand.Text.Trim();
            var res = CybersecurityLabService.SimulateRopChain(target);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine(res.ExecutionTrace);
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine("GADGET SEQUENCE SYNTHESIS:");
            for (int i = 0; i < res.GadgetChain.Count; i++)
            {
                var g = res.GadgetChain[i];
                sb.AppendLine($"  [{i + 1}] {g.Address}: {g.AssemblyOpcode,-18} | {g.RegisterEffect}");
            }
            sb.AppendLine("");
            sb.AppendLine(res.MitigationsAnalysis);
            TxtRopOutput.Text = sb.ToString();

            int totalXp = _db.AddUserXp(30);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
        }

        private void BtnSimulateAttention_Click(object sender, RoutedEventArgs e)
        {
            string sentence = string.IsNullOrWhiteSpace(TxtAttentionSentence.Text) ? "The robot completed the task because it was fast" : TxtAttentionSentence.Text.Trim();
            string query = string.IsNullOrWhiteSpace(TxtAttentionQuery.Text) ? "it" : TxtAttentionQuery.Text.Trim();

            var res = AiAcademyService.SimulateTransformerAttention(sentence, query);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(res.MathematicalTrace);
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine($"ARCHITECTURAL INSIGHT:\n{res.ArchitecturalInsight}");
            TxtAttentionOutput.Text = sb.ToString();

            int totalXp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
        }

        private void BtnSimulateQuantumGate_Click(object sender, RoutedEventArgs e)
        {
            string gate = (CmbQuantumGate.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Hadamard (H)";
            double.TryParse(TxtQuantumAlpha.Text, out double alpha);
            double.TryParse(TxtQuantumBeta.Text, out double beta);

            var res = ElectronicsSimulationService.SimulateQuantumGate(gate, alpha, beta);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(res.QuantumExplanation);
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine($"Normalized |ψ⟩ Input  : {res.InitialAlpha}|0⟩ + {res.InitialBeta}|1⟩");
            sb.AppendLine($"Evolved |ψ'⟩ Output   : {res.FinalAlpha}|0⟩ + {res.FinalBeta}|1⟩");
            sb.AppendLine($"Collapse Probability P(|0⟩): {res.ProbZero * 100:0.0}%");
            sb.AppendLine($"Collapse Probability P(|1⟩): {res.ProbOne * 100:0.0}%");
            sb.AppendLine($"Simulated Measurement : Collapsed to State |{res.MeasuredState}⟩");
            TxtQuantumGateOutput.Text = sb.ToString();

            int totalXp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
        }

        // =====================================================================
        // CRYPTOGRAPHIC CIPHER LAB HANDLERS
        // =====================================================================
        private void BtnCryptoEncrypt_Click(object sender, RoutedEventArgs e)
        {
            string algo = (CmbCryptoAlgo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Caesar Cipher";
            string input = string.IsNullOrWhiteSpace(TxtCryptoInput.Text) ? "Bhavani Secret Payload" : TxtCryptoInput.Text.Trim();
            string keyStr = string.IsNullOrWhiteSpace(TxtCryptoKey.Text) ? "3" : TxtCryptoKey.Text.Trim();

            var sb = new System.Text.StringBuilder();

            if (algo.Contains("Caesar"))
            {
                int.TryParse(keyStr, out int shift);
                var res = CryptographyLabService.ProcessCaesar(input, shift, encrypt: true);
                sb.AppendLine($"=== CAESAR CIPHER ENCRYPTION (Shift +{res.Shift}) ===");
                sb.AppendLine($"Plaintext  : {input}");
                sb.AppendLine($"Ciphertext : {res.Ciphertext}\n");
                sb.AppendLine("AUTOMATED BRUTE-FORCE DECRYPTION MATRIX (All 25 Shifts):");
                foreach (var c in res.BruteForceCandidates.Take(10)) sb.AppendLine($"  {c}");
            }
            else if (algo.Contains("Vigenère"))
            {
                var res = CryptographyLabService.ProcessVigenere(input, keyStr, encrypt: true);
                sb.AppendLine($"=== VIGENÈRE POLYALPHABETIC CIPHER (Key: '{res.Key}') ===");
                sb.AppendLine($"Plaintext  : {input}");
                sb.AppendLine($"Ciphertext : {res.ResultText}");
            }
            else if (algo.Contains("Diffie-Hellman"))
            {
                int.TryParse(keyStr, out int aPriv);
                if (aPriv <= 0) aPriv = 6;
                var res = CryptographyLabService.SimulateDiffieHellman(primeP: 23, generatorG: 5, alicePriv: aPriv, bobPriv: 15);
                sb.AppendLine(res.MathematicalTrace);
            }
            else if (algo.Contains("RSA"))
            {
                int.TryParse(keyStr, out int pVal);
                if (pVal <= 2) pVal = 61;
                var res = CryptographyLabService.SimulateRsa(pVal: pVal, qVal: 53, msgNumber: 42);
                sb.AppendLine(res.MathematicalTrace);
            }
            else if (algo.Contains("AES-GCM"))
            {
                var res = CryptographyLabService.SimulateAesGcm(input, simulateTamper: false);
                sb.AppendLine("=== AES-256-GCM AUTHENTICATED ENCRYPTION (AEAD) ===");
                sb.AppendLine($"256-bit Key       : {res.KeyHex}");
                sb.AppendLine($"96-bit Nonce (IV) : {res.IvHex}");
                sb.AppendLine($"Ciphertext        : {res.CiphertextHex}");
                sb.AppendLine($"128-bit GMAC Tag  : {res.AuthTagHex}");
                sb.AppendLine($"Integrity Status  : {(res.TagVerified ? "VERIFIED (Tamper-Proof) ✅" : "TAMPERED ❌")}\n");
                sb.AppendLine(res.SecurityAnalysis);
            }

            TxtCryptoOutput.Text = sb.ToString();
            int totalXp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
        }

        private void BtnCryptoDecrypt_Click(object sender, RoutedEventArgs e)
        {
            string algo = (CmbCryptoAlgo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Caesar Cipher";
            string input = string.IsNullOrWhiteSpace(TxtCryptoInput.Text) ? "Ekdydql VHFUHW" : TxtCryptoInput.Text.Trim();
            string keyStr = string.IsNullOrWhiteSpace(TxtCryptoKey.Text) ? "3" : TxtCryptoKey.Text.Trim();

            var sb = new System.Text.StringBuilder();

            if (algo.Contains("Caesar"))
            {
                int.TryParse(keyStr, out int shift);
                var res = CryptographyLabService.ProcessCaesar(input, shift, encrypt: false);
                sb.AppendLine($"=== CAESAR CIPHER DECRYPTION (Shift -{res.Shift}) ===");
                sb.AppendLine($"Ciphertext : {input}");
                sb.AppendLine($"Plaintext  : {res.Ciphertext}");
            }
            else if (algo.Contains("Vigenère"))
            {
                var res = CryptographyLabService.ProcessVigenere(input, keyStr, encrypt: false);
                sb.AppendLine($"=== VIGENÈRE CIPHER DECRYPTION (Key: '{res.Key}') ===");
                sb.AppendLine($"Ciphertext : {input}");
                sb.AppendLine($"Plaintext  : {res.ResultText}");
            }
            else if (algo.Contains("AES-GCM"))
            {
                var res = CryptographyLabService.SimulateAesGcm(input, simulateTamper: true);
                sb.AppendLine("=== AES-256-GCM TAMPER DETECTION TEST (Bit-Flipped Ciphertext) ===");
                sb.AppendLine($"Tampered Ciphertext: {res.CiphertextHex}");
                sb.AppendLine($"Calculated GMAC Tag: {res.AuthTagHex}");
                sb.AppendLine($"Decryption Verdict : {res.SecurityAnalysis}");
            }
            else
            {
                sb.AppendLine("Select Caesar, Vigenère, or AES-GCM to test Decryption.");
            }

            TxtCryptoOutput.Text = sb.ToString();
            int totalXp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
        }

        // =====================================================================
        // REVERSE ENGINEERING & DISASSEMBLY LAB HANDLERS
        // =====================================================================
        private void LoadRevEngPatterns()
        {
            _revEngPatterns = ReverseEngineeringService.GetStandardPatterns();
            CmbRevEngPattern.ItemsSource = _revEngPatterns.Select(p => p.PatternName).ToList();
            if (_revEngPatterns.Count > 0)
            {
                CmbRevEngPattern.SelectedIndex = 0;
            }
        }

        private void CmbRevEngPattern_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = CmbRevEngPattern.SelectedIndex;
            if (idx >= 0 && idx < _revEngPatterns.Count)
            {
                var p = _revEngPatterns[idx];
                TxtRevEngHighLevel.Text = $"// HIGH-LEVEL C/C++ SOURCE CODE:\n{p.HighLevelCode}";
                DisplayDisassembly(p);
            }
        }

        private void BtnAnalyzeDisasm_Click(object sender, RoutedEventArgs e)
        {
            int idx = CmbRevEngPattern.SelectedIndex;
            if (idx >= 0 && idx < _revEngPatterns.Count)
            {
                DisplayDisassembly(_revEngPatterns[idx]);
                int totalXp = _db.AddUserXp(25);
                TxtUserXp.Text = $"⭐ {totalXp} XP";
            }
        }

        private void DisplayDisassembly(DisassemblyPattern p)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== x86_64 MACHINE DISASSEMBLY: {p.PatternName} ===");
            sb.AppendLine("Address     Hex Opcodes              Instruction                         Explanation");
            sb.AppendLine("------------------------------------------------------------------------------------------------------------------");
            foreach (var ins in p.Instructions)
            {
                string instr = string.IsNullOrEmpty(ins.Operands) ? ins.Mnemonic : $"{ins.Mnemonic} {ins.Operands}";
                sb.AppendLine($"{ins.Address,-11} {ins.HexBytes,-24} {instr,-35} # {ins.Explanation}");
            }
            sb.AppendLine("------------------------------------------------------------------------------------------------------------------");
            sb.AppendLine($"MICROARCHITECTURE INSIGHT:\n{p.ArchitecturalExplanation}");
            TxtRevEngDisassembly.Text = sb.ToString();
        }

        // =====================================================================
        // SQL DATABASE & ALGORITHM VISUALIZER HANDLERS
        // =====================================================================
        private void BtnExecuteSql_Click(object sender, RoutedEventArgs e)
        {
            string script = TxtSqlScriptInput.Text;
            var res = PolyglotExecutionService.ExecuteSql(script);
            TxtSqlOutput.Text = $"=== SQL DATABASE RESULT ({res.ElapsedMs:F2} ms) ===\n{res.FormattedTable}";
            int totalXp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
        }

        private void BtnVisualizeAlgo_Click(object sender, RoutedEventArgs e)
        {
            string algo = (CmbAlgorithmChoice.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "QuickSort";
            string rawInput = TxtAlgorithmInput.Text.Trim();

            var nums = new List<int>();
            foreach (var part in rawInput.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(part, out int n)) nums.Add(n);
            }
            if (nums.Count == 0) nums = new List<int> { 64, 34, 25, 12, 22, 11, 90 };

            var sb = new System.Text.StringBuilder();

            if (algo.Contains("QuickSort"))
            {
                var res = AlgorithmVisualizationService.TraceQuickSort(nums.ToArray());
                sb.AppendLine($"=== {res.AlgorithmName.ToUpperInvariant()} STEP-BY-STEP TRACE ===");
                sb.AppendLine($"Initial: {res.InitialData} -> Final: {res.FinalData}");
                sb.AppendLine($"Comparisons: {res.TotalComparisons} | Swaps: {res.TotalSwaps}\n");
                foreach (var step in res.Steps)
                {
                    sb.AppendLine($"Step {step.StepNumber:D2}: {step.Description,-45} | State: {step.ArrayState} ({step.Pointers})");
                }
                sb.AppendLine($"\n{res.ComplexityInsight}");
            }
            else if (algo.Contains("Bubble"))
            {
                var res = AlgorithmVisualizationService.TraceBubbleSort(nums.ToArray());
                sb.AppendLine($"=== {res.AlgorithmName.ToUpperInvariant()} STEP-BY-STEP TRACE ===");
                sb.AppendLine($"Initial: {res.InitialData} -> Final: {res.FinalData}");
                sb.AppendLine($"Comparisons: {res.TotalComparisons} | Swaps: {res.TotalSwaps}\n");
                foreach (var step in res.Steps)
                {
                    sb.AppendLine($"Step {step.StepNumber:D2}: {step.Description,-45} | State: {step.ArrayState} ({step.Pointers})");
                }
                sb.AppendLine($"\n{res.ComplexityInsight}");
            }
            else if (algo.Contains("Binary Search"))
            {
                int target = nums.Count > 0 ? nums[nums.Count / 2] : 25;
                nums.Sort();
                var res = AlgorithmVisualizationService.TraceBinarySearch(nums.ToArray(), target);
                sb.AppendLine($"=== BINARY SEARCH LOGARITHMIC TRACE ===");
                sb.AppendLine($"Sorted Input: [{string.Join(", ", nums)}]");
                sb.AppendLine($"Target Value: {target} | Outcome: {res.FinalData}\n");
                foreach (var step in res.Steps)
                {
                    sb.AppendLine($"Step {step.StepNumber:D2}: {step.Description}");
                }
                sb.AppendLine($"\n{res.ComplexityInsight}");
            }
            else if (algo.Contains("BST"))
            {
                var res = AlgorithmVisualizationService.BuildAndTraverseBst(nums);
                sb.AppendLine("=== BINARY SEARCH TREE (BST) VISUALIZATION ===");
                sb.AppendLine(res.AsciiTree);
                sb.AppendLine($"\nIn-Order Traversal   (Sorted L-N-R) : {string.Join(", ", res.InOrder)}");
                sb.AppendLine($"Pre-Order Traversal  (N-L-R)        : {string.Join(", ", res.PreOrder)}");
                sb.AppendLine($"Post-Order Traversal (L-R-N)        : {string.Join(", ", res.PostOrder)}");
                sb.AppendLine($"\n{res.EducationalInsight}");
            }
            else if (algo.Contains("Graph"))
            {
                var res = AlgorithmVisualizationService.TraverseSampleGraph();
                sb.AppendLine("=== GRAPH ADJACENCY TRAVERSAL VISUALIZATION ===");
                sb.AppendLine(res.GraphRepresentation);
                sb.AppendLine($"\nBreadth-First Search (BFS Queue Visit Order): {string.Join(" -> ", res.BfsVisitOrder)}");
                sb.AppendLine($"Depth-First Search   (DFS Stack Visit Order): {string.Join(" -> ", res.DfsVisitOrder)}");
                sb.AppendLine($"\n{res.TraceExplanation}");
            }

            TxtAlgorithmOutput.Text = sb.ToString();
            int totalXp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
        }

        // =====================================================================
        // PHASE 3: HARDWARE & MICROARCHITECTURE LABS
        // =====================================================================
        private void BtnSimulatePipeline_Click(object sender, RoutedEventArgs e)
        {
            string scenarioText = (CmbPipelineScenario?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string key = "ideal";
            if (scenarioText.Contains("RAW") && scenarioText.Contains("Forwarding Enabled")) key = "forwarding_raw";
            else if (scenarioText.Contains("RAW") && scenarioText.Contains("No Forwarding")) key = "forwarding_raw";
            else if (scenarioText.Contains("Load-Use")) key = "load_use";
            else if (scenarioText.Contains("Control") || scenarioText.Contains("Branch")) key = "branch_hazard";

            bool forwarding = ChkPipelineForwarding?.IsChecked ?? true;
            var res = CpuPipelineService.SimulateProgram(key, forwarding);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== 5-STAGE RISC PIPELINE SIMULATION: {res.ProgramName.ToUpperInvariant()} ===");
            sb.AppendLine($"Total Cycles: {res.TotalCycles} | Instructions: {res.InstructionCount} | CPI: {res.Cpi:F2}");
            sb.AppendLine($"Pipeline Stalls (Bubbles): {res.StallsCount} | Forwarding Bypass Events: {res.ForwardingEventsCount}\n");

            // Print Cycle Headers
            sb.Append($"{"Instruction",-36} | ");
            foreach (var h in res.CycleHeaders) sb.Append($"{h,5} ");
            sb.AppendLine();
            sb.AppendLine(new string('-', 38 + (res.CycleHeaders.Count * 6)));

            for (int r = 0; r < res.GridMatrix.Count; r++)
            {
                string instrLabel = r switch
                {
                    0 => "I1: ADD R1, R2, R3",
                    1 => "I2: SUB R4, R1, R5",
                    2 => "I3: AND R6, R1, R7",
                    _ => $"I{r + 1}: Instruction {r + 1}"
                };
                sb.Append($"{instrLabel,-36} | ");
                foreach (var stage in res.GridMatrix[r])
                {
                    sb.Append($"{stage,5} ");
                }
                sb.AppendLine();
            }

            sb.AppendLine("\n--- PIPELINE HAZARD LOGS & BYPASS TRACE ---");
            foreach (var msg in res.LogMessages)
            {
                sb.AppendLine($"• {msg}");
            }

            TxtPipelineOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(30);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        private void BtnSimulateBreadboard_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(TxtBreadboardVoltage.Text, out double voltage)) voltage = 5.0;
            if (!double.TryParse(TxtBreadboardResistor.Text, out double rValue)) rValue = 330.0;

            string ledChoice = (CmbBreadboardLed?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            double ledVf = 2.0;
            bool hasLed = true;
            if (ledChoice.Contains("Green")) ledVf = 2.2;
            else if (ledChoice.Contains("Blue")) ledVf = 3.2;
            else if (ledChoice.Contains("None")) hasLed = false;

            bool isSwitchClosed = ChkBreadboardSwitch?.IsChecked ?? true;

            var comps = new List<BreadboardComponent>
            {
                new() { Name = "Power Rail Switch", Type = CircuitComponentType.Switch, IsActive = isSwitchClosed },
                new() { Name = "Current-Limiting Resistor", Type = CircuitComponentType.Resistor, Value = rValue, Unit = "Ω" }
            };

            if (hasLed)
            {
                comps.Add(new() { Name = "Indicator LED", Type = CircuitComponentType.Led, Value = ledVf, Unit = "V" });
            }

            var result = BreadboardSimulationService.SimulateSeriesCircuit(voltage, comps);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== 2D BREADBOARD CIRCUIT SOLVER & COMPONENT STATE ===");
            sb.AppendLine($"Supply Voltage: {result.SourceVoltage:F2}V | Loop Current: {result.TotalCurrentAmps * 1000:F2} mA");
            sb.AppendLine($"Equivalent Resistance: {result.TotalEquivalentResistanceOhms:F1} Ω | Total Power: {result.TotalPowerWatts * 1000:F1} mW\n");

            sb.AppendLine($"{"Component",-30} | {"Voltage Drop",-14} | {"Current",-12} | {"Status"}");
            sb.AppendLine(new string('-', 85));
            foreach (var c in result.Components)
            {
                sb.AppendLine($"{c.Name,-30} | {c.VoltageDrop:F2} V       | {c.CurrentAmps * 1000:F2} mA     | {c.Status}");
            }

            sb.AppendLine($"\nAnalysis: {result.AnalysisSummary}");

            // Also compute RC transient charging curve
            var rcPoints = BreadboardSimulationService.CalculateRcTransient(voltage, rValue, 100.0, 5);
            sb.AppendLine("\n--- RC TRANSIENT CHARGING SIMULATION (R = " + rValue + "Ω, C = 100µF, τ = " + (rValue * 100e-6 * 1000).ToString("F1") + " ms) ---");
            foreach (var pt in rcPoints)
            {
                sb.AppendLine($"t = {pt.TimeMilliseconds,6:F1} ms : Vc(t) = {pt.CapacitorVoltage,5:F2} V | I(t) = {pt.CurrentMilliamps,5:F2} mA");
            }

            TxtBreadboardOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        private void BtnExecuteMicroSketch_Click(object sender, RoutedEventArgs e)
        {
            string sketchChoice = (CmbMicroSketch?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string sketch = "blink";
            if (sketchChoice.Contains("Servo")) sketch = "servo";
            else if (sketchChoice.Contains("Analog")) sketch = "adc_read";
            else if (sketchChoice.Contains("Ultrasonic")) sketch = "ultrasonic";

            if (!byte.TryParse(TxtMicroPwm.Text, out byte pwm)) pwm = 180;
            if (!int.TryParse(TxtMicroAdc.Text, out int adc)) adc = 512;

            var state = MicrocontrollerStudioService.ExecuteSketch(sketch, pwm, adc);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== ARDUINO / ESP32 MICROCONTROLLER STUDIO [{state.BoardType}] ===");
            sb.AppendLine($"Clock: {state.ClockSpeedMhz} MHz | Vcc: {state.OperatingVoltage}V | Servo: {state.ServoAngleDegrees}° | Ultrasonic: {state.UltrasonicDistanceCm} cm\n");
            sb.AppendLine("--- FIRMWARE EXECUTION LOG ---");
            sb.AppendLine(state.ConsoleLog);

            sb.AppendLine("\n--- REAL-TIME OSCILLOSCOPE PWM SAMPLES (490 Hz Square Wave) ---");
            var wave = MicrocontrollerStudioService.GeneratePwmWaveform(pwm, 490.0, 2);
            foreach (var samp in wave.Take(8))
            {
                string bar = samp.Voltage > 2.5 ? "██████████ [HIGH 5.0V]" : "__________ [LOW 0.0V]";
                sb.AppendLine($"t = {samp.TimeMicroseconds,7:F1} µs : {bar}");
            }

            TxtMicroOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        // =====================================================================
        // PHASE 4: FRONTIER AI & COMPUTER VISION LABS
        // =====================================================================
        private void BtnTrainNeuralNet_Click(object sender, RoutedEventArgs e)
        {
            string datasetChoice = (CmbNnDataset?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string ds = "linear";
            if (datasetChoice.Contains("XOR")) ds = "xor";
            else if (datasetChoice.Contains("Circles")) ds = "circle";

            string actChoice = (CmbNnActivation?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string act = "sigmoid";
            if (actChoice.Contains("Tanh")) act = "tanh";
            else if (actChoice.Contains("ReLU")) act = "relu";

            if (!int.TryParse(TxtNnHidden.Text, out int hidden) || hidden < 1 || hidden > 8) hidden = 4;

            var netState = NeuralNetPlaygroundService.CreateNetwork(ds, act, hidden);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== NEURAL NETWORK DECISION BOUNDARY ENGINE [{netState.ActivationName.ToUpperInvariant()}] ===");
            sb.AppendLine($"Dataset: {ds.ToUpperInvariant()} | Hidden Neurons: {hidden} | Training Samples: {netState.TrainingPoints.Count}");
            sb.AppendLine($"Binary Cross-Entropy Loss: {netState.CurrentLoss:F4} | Classification Accuracy: {netState.CurrentAccuracy:F1}%\n");

            sb.AppendLine("--- 2D DECISION SURFACE GRID SAMPLING ([-2, +2] Space) ---");
            sb.AppendLine($"{"Coordinate (x1, x2)",-24} | {"Class Probability P(y=1)",-26} | {"Predicted Class"}");
            sb.AppendLine(new string('-', 65));
            foreach (var pt in netState.DecisionBoundaryGrid.Take(12))
            {
                string tag = pt.PredictedClass == 1 ? "Class 1 [● Blue]" : "Class 0 [▲ Red]";
                sb.AppendLine($"({pt.X1,5:F1}, {pt.X2,5:F1})             | {pt.Probability,6:F3}                    | {tag}");
            }

            TxtNeuralNetOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(30);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        private void BtnApplyConvolution_Click(object sender, RoutedEventArgs e)
        {
            string kernelChoice = (CmbCvKernel?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string kName = "sobel_horizontal";
            if (kernelChoice.Contains("Vertical")) kName = "sobel_vertical";
            else if (kernelChoice.Contains("Sharpen")) kName = "sharpen";
            else if (kernelChoice.Contains("Ridge")) kName = "ridge_laplacian";
            else if (kernelChoice.Contains("Gaussian")) kName = "gaussian_blur";

            string patChoice = (CmbCvPattern?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            string pattern = "box";
            if (patChoice.Contains("Cross")) pattern = "cross";
            else if (patChoice.Contains("Diagonal")) pattern = "diagonal";

            var res = ComputerVisionLabService.ApplyConvolution(kName, pattern);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== COMPUTER VISION 3X3 SPATIAL CONVOLUTION [{res.KernelName.ToUpperInvariant()}] ===");
            sb.AppendLine($"Input Image: {res.ImageWidth}x{res.ImageHeight} Grayscale | Pattern: {pattern}\n");

            sb.AppendLine("--- 3X3 CONVOLUTION KERNEL MATRIX ---");
            for (int r = 0; r < 3; r++)
            {
                sb.Append("  [ ");
                for (int c = 0; c < 3; c++) sb.Append($"{res.KernelMatrix[r, c],6:F2} ");
                sb.AppendLine("]");
            }

            sb.AppendLine("\n--- SAMPLE FILTERED PIXEL BREAKDOWN ---");
            foreach (var step in res.SampleSteps)
            {
                sb.AppendLine($"• {step.MathExpression}");
            }

            sb.AppendLine("\n--- 2X2 MAX POOLING DOWNSAMPLED (4x4 Feature Map) ---");
            int ph = res.PooledImage.GetLength(0);
            int pw = res.PooledImage.GetLength(1);
            for (int r = 0; r < ph; r++)
            {
                sb.Append("  [ ");
                for (int c = 0; c < pw; c++) sb.Append($"{res.PooledImage[r, c],4} ");
                sb.AppendLine("]");
            }

            TxtConvolutionOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(30);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        private void BtnExecuteRag_Click(object sender, RoutedEventArgs e)
        {
            string query = TxtRagQuery.Text;
            var ragRes = VectorRagService.ExecuteRagQuery(query, 2);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== OFFLINE VECTOR SEARCH & RAG (RETRIEVAL-AUGMENTED GENERATION) ===");
            sb.AppendLine($"Natural Query: \"{ragRes.Query}\"\n");

            sb.AppendLine("--- TOP RETRIEVED KNOWLEDGE PASSAGES (COSINE SIMILARITY) ---");
            foreach (var match in ragRes.TopMatches)
            {
                sb.AppendLine($"Rank #{match.Rank} [Score: {match.CosineSimilarity:P1}] - [{match.Document.Category}] {match.Document.Title}");
                sb.AppendLine($"Excerpt: {match.Document.Content}\n");
            }

            sb.AppendLine("--- GROUNDED SYNTHESIS RESULT ---");
            sb.AppendLine(ragRes.SynthesizedAnswer);

            TxtRagOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(30);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        // =====================================================================
        // PHASE 5: NETWORKING TOPOLOGY & CLOUD INFRASTRUCTURE
        // =====================================================================
        private void BtnTracePacket_Click(object sender, RoutedEventArgs e)
        {
            string src = TxtTopoSrc.Text;
            string dst = TxtTopoDst.Text;
            string proto = (CmbTopoProto?.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "HTTP";
            bool block = ChkFirewallBlock?.IsChecked ?? false;

            var res = NetworkTopologyService.TracePacket(src, dst, proto, block);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== MULTI-HOP PACKET TRACER & TOPOLOGY ROUTING [{res.Protocol}] ===");
            sb.AppendLine($"Path: {res.SourceIp} -> {res.DestinationIp} | Status: {(res.IsSuccessful ? "DELIVERED (200 OK)" : "DROPPED (Firewall Filter)")}\n");

            foreach (var hop in res.Hops)
            {
                string status = hop.IsDropped ? "[DROPPED]" : "[FORWARDED]";
                sb.AppendLine($"Hop {hop.StepNumber}: {hop.FromDevice} -> {hop.ToDevice} ({hop.Layer}) {status}");
                sb.AppendLine($"  Action: {hop.ActionDescription}");
                if (!string.IsNullOrEmpty(hop.DropReason))
                {
                    sb.AppendLine($"  Reason: {hop.DropReason}");
                }
                sb.AppendLine();
            }

            sb.AppendLine($"Summary: {res.Summary}");
            TxtNetworkTopoOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        private void BtnResolveDns_Click(object sender, RoutedEventArgs e)
        {
            string domain = TxtDnsDomain.Text;
            bool cacheHit = ChkDnsCacheHit?.IsChecked ?? false;

            var res = DnsResolutionService.ResolveDomain(domain, cacheHit);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== HIERARCHICAL DNS RESOLUTION TRACE [{res.DomainName}] ===");
            sb.AppendLine($"Resolved IPv4: {res.ResolvedIp} | Total Resolution Latency: {res.TotalLatencyMs} ms\n");

            foreach (var step in res.Steps)
            {
                sb.AppendLine($"Step {step.StepNumber}: {step.QueryServerType} ({step.ServerName})");
                sb.AppendLine($"  Query: {step.Question} -> Response: {step.ResponseType} ({step.ResponseData})");
                sb.AppendLine($"  TTL: {step.TtlSeconds}s | {step.Explanation}\n");
            }

            sb.AppendLine($"Outcome: {res.Summary}");
            TxtDnsOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(25);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        private void BtnScheduleK8sPod_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtK8sCpu.Text, out int cpu)) cpu = 1;
            if (!int.TryParse(TxtK8sRam.Text, out int ram)) ram = 2;
            bool gpuToleration = ChkGpuToleration?.IsChecked ?? false;

            var pod = new K8sPod
            {
                Name = "student-workload-" + Guid.NewGuid().ToString("N")[..4],
                RequestCpuCores = cpu,
                RequestMemoryGb = ram
            };

            if (gpuToleration)
            {
                pod.Tolerations.Add("specialized=gpu:NoSchedule");
            }

            var decision = K8sSchedulerService.SchedulePod(pod);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== KUBERNETES POD SCHEDULER ENGINE (kube-scheduler) ===");
            sb.AppendLine($"Pod: {decision.PodName} | Req CPU: {cpu} cores | Req RAM: {ram} GB | Target: {decision.TargetNode}\n");

            sb.AppendLine("--- 2-STAGE SCHEDULING CYCLE (FILTERING & SCORING) ---");
            foreach (var ev in decision.Evaluations)
            {
                string status = ev.FilterPassed ? "[PASSED PREDICATES]" : "[FAILED PREDICATES]";
                sb.AppendLine($"Node: {ev.NodeName} {status}");
                sb.AppendLine($"  Filter Reason: {ev.FilterReason}");
                sb.AppendLine($"  Score: {ev.Score}/100 -> {ev.ScoreReason}\n");
            }

            sb.AppendLine($"Decision: {decision.Summary}");
            TxtK8sOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(30);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        // =====================================================================
        // PHASE 6: GAMIFICATION, CERTIFICATION & PORTABILITY
        // =====================================================================
        private void RefreshBadgesList()
        {
            var badges = CertificationExamService.GetAllBadges();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== BHAVANI TECHNOLOGY MASTERY SKILL TREE (12 BADGES) ===\n");

            int unlockedCount = 0;
            foreach (var b in badges)
            {
                if (b.IsUnlocked) unlockedCount++;
                string status = b.IsUnlocked ? "★ UNLOCKED ★" : "[LOCKED - Complete Lessons]";
                string dateStr = b.UnlockedAt.HasValue ? $"({b.UnlockedAt.Value:yyyy-MM-dd})" : "";
                sb.AppendLine($"{b.IconEmoji} {b.Title,-26} | [{b.Category,-18}] | {status} {dateStr}");
                sb.AppendLine($"   Description: {b.Description}\n");
            }

            sb.AppendLine($"Total Badges Unlocked: {unlockedCount} / {badges.Count} ({(double)unlockedCount / badges.Count:P0})");
            TxtBadgesOutput.Text = sb.ToString();
        }

        private void BtnRefreshBadges_Click(object sender, RoutedEventArgs e)
        {
            RefreshBadgesList();
        }

        private void BtnTakeCertificationExam_Click(object sender, RoutedEventArgs e)
        {
            string candidateName = TxtExamCandidateName.Text;
            var answers = new Dictionary<int, int>
            {
                { 1, 1 }, // OS Page fault
                { 2, 1 }, // RSA Factorization
                { 3, 2 }, // EX stage
                { 4, 2 }, // Stack canary
                { 5, 2 }, // ARP
                { 6, 1 }, // 10.0 mA
                { 7, 2 }, // ReLU
                { 8, 1 }  // Filtering
            };

            var grading = CertificationExamService.GradeExam(candidateName, answers);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== BHAVANI TECHNOLOGY COMPREHENSIVE CERTIFICATION EXAM ===");
            sb.AppendLine($"Candidate: {candidateName} | Score: {grading.ScorePercent}% ({grading.CorrectCount}/{grading.TotalQuestions} Correct)");
            sb.AppendLine($"Status: {(grading.Passed ? "PASSED WITH HONORS" : "DID NOT PASS")}\n");

            sb.AppendLine("--- QUESTION-BY-QUESTION AUDIT ---");
            foreach (var r in grading.DetailedReviews)
            {
                sb.AppendLine(r);
            }

            if (grading.Certificate != null)
            {
                sb.AppendLine("\n" + grading.Certificate.AsciiCertificateText);
            }

            TxtExamOutput.Text = sb.ToString();
            int xp = _db.AddUserXp(100);
            TxtUserXp.Text = $"⭐ {xp} XP";
        }

        private void BtnExportProgressJson_Click(object sender, RoutedEventArgs e)
        {
            var profile = new StudentProgressProfile
            {
                StudentName = string.IsNullOrWhiteSpace(TxtExamCandidateName.Text) ? "Dharmesh Varia" : TxtExamCandidateName.Text.Trim(),
                Level = _currentUser?.CurrentLevel ?? 5,
                TotalXp = _currentUser?.TotalXP ?? 2500,
                CompletedLessonIds = new List<string> { "lesson_1", "lesson_2", "lesson_3", "lesson_4", "lesson_5" },
                UnlockedBadgeIds = new List<string> { "badge_byte", "badge_kernel", "badge_polyglot", "badge_crypto" },
                Certificates = new List<TechnologyCertificate>
                {
                    CertificationExamService.GenerateCertificate(TxtExamCandidateName.Text, 100)
                }
            };

            string json = ProgressPortabilityService.ExportProgressToJson(profile);
            TxtProgressJsonOutput.Text = json;
        }

        private void BtnImportProgressJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string json = TxtProgressJsonOutput.Text;
                var profile = ProgressPortabilityService.ImportProgressFromJson(json);
                MessageBox.Show($"Progress JSON Imported Successfully!\nStudent: {profile.StudentName}\nLevel: {profile.Level}\nTotal XP: {profile.TotalXp}\nCertificates: {profile.Certificates.Count}", "Progress Portability", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
                int xp = _db.AddUserXp(50);
                TxtUserXp.Text = $"⭐ {xp} XP";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import Error: {ex.Message}", "Invalid Progress JSON", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLoadDefaultProfile_Click(object sender, RoutedEventArgs e)
        {
            var def = ProgressPortabilityService.CreateDefaultProfile("Dharmesh Varia");
            TxtProgressJsonOutput.Text = ProgressPortabilityService.ExportProgressToJson(def);
        }

        // =========================================================================
        // PRACTICAL EXAMINATIONS METHODS
        // =========================================================================
        private void LoadPracticalExam(string lessonId)
        {
            if (TxtPracticalTitle == null) return;

            _currentPracticalExam = _db.GetPracticalExamForLesson(lessonId);
            if (_currentPracticalExam != null)
            {
                TxtPracticalTitle.Text = _currentPracticalExam.Title;
                TxtPracticalEvalType.Text = _currentPracticalExam.EvaluationType;
                TxtPracticalScenario.Text = _currentPracticalExam.Scenario;
                TxtPracticalTasks.Text = _currentPracticalExam.TaskInstructions;
                TxtPracticalCode.Text = _currentPracticalExam.StarterCode;
                TxtPracticalGradeStatus.Text = "Grading Terminal: Ready for evaluation.";
                TxtPracticalGradeStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#94A3B8")!;
                TxtPracticalFeedback.Text = "Click 'Run & Grade Practical Exam' to test assertions against the execution engine.";
            }
            else
            {
                TxtPracticalTitle.Text = "No practical exam registered for this lesson.";
                TxtPracticalEvalType.Text = "--";
                TxtPracticalScenario.Text = "Challenge coming soon.";
                TxtPracticalTasks.Text = "N/A";
                TxtPracticalCode.Text = "// No starter code available";
                TxtPracticalGradeStatus.Text = "N/A";
                TxtPracticalFeedback.Text = "";
            }
        }

        private void BtnRunPracticalExam_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPracticalExam == null || _selectedLesson == null)
            {
                MessageBox.Show("Please select a valid lesson with an active practical exam.", "Practical Exam", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
                return;
            }

            string submission = TxtPracticalCode.Text;
            var result = PracticalExamService.Evaluate(_selectedLesson.Id, submission);

            TxtPracticalFeedback.Text = string.Join("\n", result.TestOutputLogs);

            if (result.Passed)
            {
                TxtPracticalGradeStatus.Text = $"GRADE: {result.Score} / {result.MaxScore} (PASSED ✅)";
                TxtPracticalGradeStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#4ADE80")!;

                _db.SavePracticalExamProgress(1, _selectedLesson.Id, result.Score, result.XpEarned);
                int newXp = _db.AddUserXp(result.XpEarned);
                TxtUserXp.Text = $"⭐ {newXp} XP";
                LoadUserData();
                UpdateDashboardHero();

                MessageBox.Show(
                    $"🏆 PRACTICAL EXAMINATION PASSED!\n\n" +
                    $"Exam: {_currentPracticalExam.Title}\n" +
                    $"Score: {result.Score}/{result.MaxScore}\n" +
                    $"Reward: +{result.XpEarned} XP Credited!\n\n" +
                    $"Your practical competence is verified and saved in your offline record.",
                    "Practical Exam Certified! ✅",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
            else
            {
                TxtPracticalGradeStatus.Text = $"GRADE: {result.Score} / {result.MaxScore} (FAILED ❌)";
                TxtPracticalGradeStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444")!;
            }
        }

        private void BtnResetPracticalCode_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPracticalExam != null)
            {
                TxtPracticalCode.Text = _currentPracticalExam.StarterCode;
                TxtPracticalGradeStatus.Text = "Grading Terminal: Starter code reset.";
                TxtPracticalGradeStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#94A3B8")!;
            }
        }

        private void BtnShowPracticalHint_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPracticalExam != null)
            {
                MessageBox.Show($"💡 PRACTICAL EXAM HINT:\n\n{_currentPracticalExam.Hint}", "Exam Hint", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        // =========================================================================
        // LEARN WITH GAMING ARCADE (5 INTERACTIVE TECH GAMES)
        // =========================================================================
        private void InitArcadeGames()
        {
            // Game 1: Maze Runner
            if (_currentMazeState == null)
            {
                _currentMazeState = ArcadeGameEngine.GetMazeLevel(1);
                RenderMazeCanvas(_currentMazeState);
            }

            // Game 2: Firewall Sentry
            if (_firewallPackets.Count == 0)
            {
                LoadPacketWave(1);
            }

            // Game 5: Binary Blitz
            InitBinaryBlitz();
        }

        // GAME 1: BYTEBOT CODE MAZE RUNNER
        private void CmbMazeLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbMazeLevel == null || CnvMazeGrid == null || TxtMazeStatus == null || TxtMazeGemsCount == null || TxtMazeLog == null) return;
            int lvl = CmbMazeLevel.SelectedIndex + 1;
            _currentMazeState = ArcadeGameEngine.GetMazeLevel(lvl);
            RenderMazeCanvas(_currentMazeState);
            TxtMazeStatus.Text = _currentMazeState.Message;
            TxtMazeGemsCount.Text = $" | Chips: {_currentMazeState.GemsCollected}";
            TxtMazeLog.Text = $"Level {lvl} loaded. Program ByteBot to reach the CPU!";
        }

        private void BtnResetMaze_Click(object sender, RoutedEventArgs e)
        {
            int lvl = (CmbMazeLevel?.SelectedIndex ?? 0) + 1;
            _currentMazeState = ArcadeGameEngine.GetMazeLevel(lvl);
            RenderMazeCanvas(_currentMazeState);
            TxtMazeStatus.Text = _currentMazeState.Message;
            TxtMazeGemsCount.Text = $" | Chips: 0";
            TxtMazeLog.Text = "Level reset to starting state.";
        }

        private void BtnMazeCmd_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string cmd)
            {
                TxtMazeCommands.AppendText(cmd + Environment.NewLine);
            }
        }

        private void BtnRunMazeProgram_Click(object sender, RoutedEventArgs e)
        {
            if (_currentMazeState == null) return;

            string code = TxtMazeCommands.Text;
            var (newState, logs) = ArcadeGameEngine.RunBotCommands(_currentMazeState, code);
            _currentMazeState = newState;
            RenderMazeCanvas(_currentMazeState);

            TxtMazeLog.Text = string.Join(Environment.NewLine, logs);
            TxtMazeGemsCount.Text = $" | Chips: {_currentMazeState.GemsCollected}";

            if (_currentMazeState.Completed)
            {
                TxtMazeStatus.Text = "🎉 VICTORY! LEVEL COMPLETE! (+100 XP)";
                int newXp = _db.AddUserXp(100);
                TxtUserXp.Text = $"⭐ {newXp} XP";
                MessageBox.Show("🎉 ByteBot reached the Mainframe!\nLevel Completed! +100 XP awarded!", "Arcade Victory! 🤖", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
            else
            {
                TxtMazeStatus.Text = "Execution finished. Adjust code to guide bot to target!";
            }
        }

        private void RenderMazeCanvas(MazeGameState state)
        {
            if (CnvMazeGrid == null) return;
            CnvMazeGrid.Children.Clear();

            double cellW = CnvMazeGrid.Width / state.GridWidth;
            double cellH = CnvMazeGrid.Height / state.GridHeight;

            // Draw Grid Cells
            for (int x = 0; x < state.GridWidth; x++)
            {
                for (int y = 0; y < state.GridHeight; y++)
                {
                    var rect = new Rectangle
                    {
                        Width = cellW - 2,
                        Height = cellH - 2,
                        RadiusX = 4,
                        RadiusY = 4,
                        Fill = new SolidColorBrush(Color.FromRgb(0x0F, 0x17, 0x2A)),
                        Stroke = new SolidColorBrush(Color.FromRgb(0x1E, 0x29, 0x3B)),
                        StrokeThickness = 1
                    };
                    Canvas.SetLeft(rect, x * cellW + 1);
                    Canvas.SetTop(rect, y * cellH + 1);
                    CnvMazeGrid.Children.Add(rect);
                }
            }

            // Draw Walls (Firewalls)
            foreach (var (wx, wy) in state.Walls)
            {
                var wall = new Rectangle
                {
                    Width = cellW - 2,
                    Height = cellH - 2,
                    RadiusX = 4,
                    RadiusY = 4,
                    Fill = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44)),
                    Stroke = new SolidColorBrush(Color.FromRgb(0x99, 0x1B, 0x1B)),
                    StrokeThickness = 2
                };
                Canvas.SetLeft(wall, wx * cellW + 1);
                Canvas.SetTop(wall, wy * cellH + 1);
                CnvMazeGrid.Children.Add(wall);
            }

            // Draw Memory Gems / Chips
            foreach (var (gx, gy) in state.Gems)
            {
                var gem = new Ellipse
                {
                    Width = cellW * 0.45,
                    Height = cellH * 0.45,
                    Fill = new SolidColorBrush(Color.FromRgb(0x38, 0xBD, 0xF8)),
                    Stroke = new SolidColorBrush(Color.FromRgb(0xBA, 0xE6, 0xFD)),
                    StrokeThickness = 2
                };
                Canvas.SetLeft(gem, gx * cellW + (cellW * 0.275));
                Canvas.SetTop(gem, gy * cellH + (cellH * 0.275));
                CnvMazeGrid.Children.Add(gem);
            }

            // Draw CPU Mainframe Target
            var cpuTarget = new Rectangle
            {
                Width = cellW - 4,
                Height = cellH - 4,
                RadiusX = 6,
                RadiusY = 6,
                Fill = new SolidColorBrush(Color.FromRgb(0xFA, 0xCC, 0x15)),
                Stroke = new SolidColorBrush(Color.FromRgb(0xCA, 0x8A, 0x04)),
                StrokeThickness = 2
            };
            Canvas.SetLeft(cpuTarget, state.TargetX * cellW + 2);
            Canvas.SetTop(cpuTarget, state.TargetY * cellH + 2);
            CnvMazeGrid.Children.Add(cpuTarget);

            // Draw ByteBot
            var botBody = new Ellipse
            {
                Width = cellW * 0.65,
                Height = cellH * 0.65,
                Fill = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)),
                Stroke = Brushes.White,
                StrokeThickness = 2
            };
            Canvas.SetLeft(botBody, state.BotX * cellW + (cellW * 0.175));
            Canvas.SetTop(botBody, state.BotY * cellH + (cellH * 0.175));
            CnvMazeGrid.Children.Add(botBody);
        }

        // GAME 2: CYBER DEFENSE FIREWALL SENTRY
        private void LoadPacketWave(int wave)
        {
            _firewallWave = wave;
            TxtFirewallWave.Text = $"Wave {wave}";
            _firewallPackets = ArcadeGameEngine.GeneratePacketWave(wave);
            RefreshFirewallPacketList();
            TxtFirewallLog.Text = $"Wave {wave} started! Incoming network packet traffic detected. Analyze and triage threats!";
        }

        private void RefreshFirewallPacketList()
        {
            LstFirewallPackets.Items.Clear();
            foreach (var p in _firewallPackets)
            {
                LstFirewallPackets.Items.Add($"[{p.Protocol}] {p.SourceIp}:{p.Port} -> {p.DestIp} | Payload: {p.Payload}");
            }
        }

        private void BtnNextPacketWave_Click(object sender, RoutedEventArgs e)
        {
            LoadPacketWave(_firewallWave + 1);
        }

        private void BtnDropPacket_Click(object sender, RoutedEventArgs e)
        {
            int idx = LstFirewallPackets.SelectedIndex;
            if (idx >= 0 && idx < _firewallPackets.Count)
            {
                var pkt = _firewallPackets[idx];
                var (scoreDelta, healthDelta, feedback) = ArcadeGameEngine.EvaluateFirewallAction(pkt, true);
                _firewallScore = Math.Max(0, _firewallScore + scoreDelta);
                _serverHealth = Math.Clamp(_serverHealth + healthDelta, 0, 100);

                TxtFirewallScore.Text = $"{_firewallScore} PTS";
                TxtServerHealthPct.Text = $"{_serverHealth}%";
                PbServerHealth.Value = _serverHealth;
                TxtFirewallLog.Text = feedback;

                _firewallPackets.RemoveAt(idx);
                RefreshFirewallPacketList();

                if (_serverHealth <= 0)
                {
                    MessageBox.Show("🔴 SERVER INTEGRITY BREACHED! Server went offline. Restoring backup...", "Breach Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                    _serverHealth = 100;
                    PbServerHealth.Value = 100;
                    TxtServerHealthPct.Text = "100%";
                }
            }
            else
            {
                MessageBox.Show("Select a packet from the stream first.", "Firewall", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        private void BtnAllowPacket_Click(object sender, RoutedEventArgs e)
        {
            int idx = LstFirewallPackets.SelectedIndex;
            if (idx >= 0 && idx < _firewallPackets.Count)
            {
                var pkt = _firewallPackets[idx];
                var (scoreDelta, healthDelta, feedback) = ArcadeGameEngine.EvaluateFirewallAction(pkt, false);
                _firewallScore = Math.Max(0, _firewallScore + scoreDelta);
                _serverHealth = Math.Clamp(_serverHealth + healthDelta, 0, 100);

                TxtFirewallScore.Text = $"{_firewallScore} PTS";
                TxtServerHealthPct.Text = $"{_serverHealth}%";
                PbServerHealth.Value = _serverHealth;
                TxtFirewallLog.Text = feedback;

                _firewallPackets.RemoveAt(idx);
                RefreshFirewallPacketList();
            }
            else
            {
                MessageBox.Show("Select a packet from the stream first.", "Firewall", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        private void BtnBlockPort4444_Click(object sender, RoutedEventArgs e)
        {
            int dropped = _firewallPackets.RemoveAll(p => p.Port == 4444);
            _firewallScore += dropped * 100;
            TxtFirewallScore.Text = $"{_firewallScore} PTS";
            RefreshFirewallPacketList();
            TxtFirewallLog.Text = $"🛡️ AUTOMATED RULE: Dropped {dropped} Metasploit packet(s) on Port 4444! (+{dropped * 100} PTS)";
        }

        private void BtnBlockPort23_Click(object sender, RoutedEventArgs e)
        {
            int dropped = _firewallPackets.RemoveAll(p => p.Port == 23);
            _firewallScore += dropped * 100;
            TxtFirewallScore.Text = $"{_firewallScore} PTS";
            RefreshFirewallPacketList();
            TxtFirewallLog.Text = $"🛡️ AUTOMATED RULE: Dropped {dropped} Mirai Telnet packet(s) on Port 23! (+{dropped * 100} PTS)";
        }

        // GAME 3: CIRCUIT LOGIC REACTOR
        private void BtnTestReactor_Click(object sender, RoutedEventArgs e)
        {
            bool swA = ChkSwA.IsChecked == true;
            bool swB = ChkSwB.IsChecked == true;
            bool swC = ChkSwC.IsChecked == true;
            bool swD = ChkSwD.IsChecked == true;

            string g1 = (CmbGate1.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "AND";
            string g2 = (CmbGate2.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "OR";
            string g3 = (CmbGate3.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "XOR";

            var (corePower, out1, out2, diag) = ArcadeGameEngine.EvaluateReactorLogic(swA, swB, swC, swD, g1, g2, g3);

            TxtReactorLog.Text = diag;
            if (corePower)
            {
                ElpReactorCore.Fill = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81));
                TxtReactorStatus.Text = "ONLINE (5.0V) ⚡";
                TxtReactorStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x4A, 0xDE, 0x80));
                int newXp = _db.AddUserXp(100);
                TxtUserXp.Text = $"⭐ {newXp} XP";
            }
            else
            {
                ElpReactorCore.Fill = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
                TxtReactorStatus.Text = "OFFLINE (0V)";
                TxtReactorStatus.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
            }
        }

        // GAME 4: SQL DUNGEON QUEST
        private void CmbSqlQuestLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSqlQuestLevel == null || TxtSqlQuestDesc == null || TxtSqlDungeonQuery == null) return;
            int quest = CmbSqlQuestLevel.SelectedIndex + 1;
            switch (quest)
            {
                case 1:
                    TxtSqlQuestDesc.Text = "Quest 1: Scout Flame Dragon's weakness using SQL SELECT query!";
                    TxtSqlDungeonQuery.Text = "SELECT * FROM Monsters WHERE Element = 'FIRE';";
                    break;
                case 2:
                    TxtSqlQuestDesc.Text = "Quest 2: Equip Frost Spellbook via SQL UPDATE to fight the dragon!";
                    TxtSqlDungeonQuery.Text = "UPDATE Inventory SET Equipped = 1 WHERE ItemName LIKE '%Frost%';";
                    break;
                case 3:
                    TxtSqlQuestDesc.Text = "Quest 3: Unlock Dragon Chamber Room 2 Door via SQL UPDATE!";
                    TxtSqlDungeonQuery.Text = "UPDATE Doors SET IsLocked = 0 WHERE RoomId = 2;";
                    break;
            }
        }

        private void BtnCastSqlSpell_Click(object sender, RoutedEventArgs e)
        {
            int quest = (CmbSqlQuestLevel?.SelectedIndex ?? 0) + 1;
            string sql = TxtSqlDungeonQuery.Text;
            var (success, msg, visual) = ArcadeGameEngine.ExecuteDungeonQuery(sql, quest);

            TxtSqlQuestResult.Text = msg;
            TxtSqlDungeonVisual.Text = visual;

            if (success)
            {
                int newXp = _db.AddUserXp(100);
                TxtUserXp.Text = $"⭐ {newXp} XP";
                MessageBox.Show($"⚔️ SQL Quest {quest} Succeeded!\n+100 XP awarded!", "Dungeon Victory", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        // GAME 5: BINARY BLITZ BIT SHIFTER
        private void InitBinaryBlitz()
        {
            _binaryBits = new bool[8];
            BtnNewBinaryTarget_Click(this, new RoutedEventArgs());
            UpdateBinaryBitsDisplay();
        }

        private void BtnNewBinaryTarget_Click(object sender, RoutedEventArgs e)
        {
            var (dec, hex, bin) = ArcadeGameEngine.GenerateBinaryTarget();
            _binaryTargetVal = dec;
            TxtBinaryTarget.Text = $"Target: {dec} (Hex: {hex})";
            TxtBinaryFeedback.Text = "Toggle 8-bit switches to match the target.";
        }

        private void BtnToggleBit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int bitIndex))
            {
                _binaryBits[7 - bitIndex] = !_binaryBits[7 - bitIndex];
                UpdateBinaryBitsDisplay();
            }
        }

        private void UpdateBinaryBitsDisplay()
        {
            Button[] btns = { BtnBit7, BtnBit6, BtnBit5, BtnBit4, BtnBit3, BtnBit2, BtnBit1, BtnBit0 };
            for (int i = 0; i < 8; i++)
            {
                bool isSet = _binaryBits[i];
                btns[i].Content = isSet ? "1" : "0";
                btns[i].Background = isSet ? new SolidColorBrush(Color.FromRgb(0x05, 0x96, 0x69)) : new SolidColorBrush(Color.FromRgb(0x1E, 0x29, 0x3B));
                btns[i].Foreground = isSet ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x4A, 0xDE, 0x80));
            }

            int val = ArcadeGameEngine.CalculateByteFromBits(_binaryBits);
            string binStr = Convert.ToString(val, 2).PadLeft(8, '0');
            TxtBinaryCurrentVal.Text = $"Current Value: {val} (Binary: {binStr})";
        }

        private void BtnShiftLeft_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 7; i++) _binaryBits[i] = _binaryBits[i + 1];
            _binaryBits[7] = false;
            UpdateBinaryBitsDisplay();
        }

        private void BtnShiftRight_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 7; i > 0; i--) _binaryBits[i] = _binaryBits[i - 1];
            _binaryBits[0] = false;
            UpdateBinaryBitsDisplay();
        }

        private void BtnSubmitBinaryPattern_Click(object sender, RoutedEventArgs e)
        {
            int current = ArcadeGameEngine.CalculateByteFromBits(_binaryBits);
            if (current == _binaryTargetVal)
            {
                _binaryBlitzScore += 50;
                TxtBinaryScore.Text = $"{_binaryBlitzScore} PTS";
                TxtBinaryFeedback.Text = $"🎉 MATCH! You matched {_binaryTargetVal}! (+50 PTS / +50 XP)";
                int newXp = _db.AddUserXp(50);
                TxtUserXp.Text = $"⭐ {newXp} XP";
                BtnNewBinaryTarget_Click(this, new RoutedEventArgs());
            }
            else
            {
                TxtBinaryFeedback.Text = $"❌ Mismatch! Current is {current}, but target is {_binaryTargetVal}. Try again!";
            }
        }

        // =========================================================================
        // ZERO TO HERO IN COMPLETE TECHNOLOGY CONTROLLERS
        // =========================================================================
        private int _diagCurrentIndex = 0;
        private int[] _diagAnswers = new int[10];
        private HeroCapstoneProject? _currentSelectedCapstone;

        private void InitZeroToHero()
        {
            RenderRoadmapStages();
            InitCapstoneSelector();
            LoadDiagnosticQuestion(_diagCurrentIndex);
            UpdateHeroRadarUI();
        }

        private void RenderRoadmapStages()
        {
            if (StackRoadmapStages == null) return;
            StackRoadmapStages.Children.Clear();

            var stages = ZeroToHeroService.GetStageMilestones();
            int userXp = _currentUser?.TotalXP ?? 0;

            foreach (var stage in stages)
            {
                bool isUnlocked = userXp >= stage.RequiredXp;
                var card = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x0F, 0x17, 0x2A)),
                    CornerRadius = new CornerRadius(8),
                    BorderBrush = new SolidColorBrush(isUnlocked ? Color.FromRgb(0x02, 0x84, 0xC7) : Color.FromRgb(0x33, 0x41, 0x55)),
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(16),
                    Margin = new Thickness(0, 0, 0, 12)
                };

                var sp = new StackPanel();

                var headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var titleStack = new StackPanel();
                var titleTxt = new TextBlock
                {
                    Text = stage.TierTitle,
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    Foreground = isUnlocked ? new SolidColorBrush(Color.FromRgb(0x38, 0xBD, 0xF8)) : new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8))
                };
                var badgeTxt = new TextBlock
                {
                    Text = $"{stage.BadgeName}  •  Required XP: {stage.RequiredXp}",
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = isUnlocked ? new SolidColorBrush(Color.FromRgb(0x4A, 0xDE, 0x80)) : new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B)),
                    Margin = new Thickness(0, 2, 0, 0)
                };
                titleStack.Children.Add(titleTxt);
                titleStack.Children.Add(badgeTxt);
                headerGrid.Children.Add(titleStack);

                var statusBorder = new Border
                {
                    Background = new SolidColorBrush(isUnlocked ? Color.FromRgb(0x05, 0x96, 0x69) : Color.FromRgb(0x33, 0x41, 0x55)),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(8, 4, 8, 4),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(statusBorder, 1);
                statusBorder.Child = new TextBlock
                {
                    Text = isUnlocked ? "ACTIVE / UNLOCKED ✅" : "LOCKED 🔒",
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                };
                headerGrid.Children.Add(statusBorder);
                sp.Children.Add(headerGrid);

                var descTxt = new TextBlock
                {
                    Text = stage.Summary,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xCB, 0xD5, 0xE1)),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 8, 0, 8)
                };
                sp.Children.Add(descTxt);

                var topicsPanel = new WrapPanel { Margin = new Thickness(0, 0, 0, 8) };
                foreach (var topic in stage.KeyTopics)
                {
                    var tBorder = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x29, 0x3B)),
                        CornerRadius = new CornerRadius(4),
                        Padding = new Thickness(6, 2, 6, 2),
                        Margin = new Thickness(0, 0, 6, 4)
                    };
                    tBorder.Child = new TextBlock
                    {
                        Text = $"• {topic}",
                        FontSize = 10,
                        Foreground = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8))
                    };
                    topicsPanel.Children.Add(tBorder);
                }
                sp.Children.Add(topicsPanel);

                var capstoneRow = new Grid();
                capstoneRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                capstoneRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var capTxt = new TextBlock
                {
                    Text = $"🎯 {stage.CapstoneTitle}",
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                capstoneRow.Children.Add(capTxt);

                var btnJump = new Button
                {
                    Content = "Launch Capstone Studio 🚀",
                    Background = new SolidColorBrush(Color.FromRgb(0x02, 0x84, 0xC7)),
                    Foreground = Brushes.White,
                    Padding = new Thickness(10, 4, 10, 4),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand
                };
                Grid.SetColumn(btnJump, 1);
                btnJump.Click += (s, e) =>
                {
                    if (TabsZeroToHero != null) TabsZeroToHero.SelectedIndex = 2; // Jump to Capstone Builder tab
                };
                capstoneRow.Children.Add(btnJump);

                sp.Children.Add(capstoneRow);
                card.Child = sp;
                StackRoadmapStages.Children.Add(card);
            }
        }

        // --- DIAGNOSTIC QUIZ ---
        private void LoadDiagnosticQuestion(int index)
        {
            var questions = ZeroToHeroService.GetDiagnosticQuestions();
            if (index < 0 || index >= questions.Count) return;

            var q = questions[index];
            if (TxtDiagPillarBadge != null) TxtDiagPillarBadge.Text = $"[Domain: {q.PillarAssessed}]";
            if (TxtDiagQuestionText != null) TxtDiagQuestionText.Text = $"Question {q.Id}: {q.Question}";

            if (RadDiagOptA != null) { RadDiagOptA.Content = q.Options[0]; RadDiagOptA.IsChecked = false; }
            if (RadDiagOptB != null) { RadDiagOptB.Content = q.Options[1]; RadDiagOptB.IsChecked = false; }
            if (RadDiagOptC != null) { RadDiagOptC.Content = q.Options[2]; RadDiagOptC.IsChecked = false; }
            if (RadDiagOptD != null) { RadDiagOptD.Content = q.Options[3]; RadDiagOptD.IsChecked = false; }

            if (PbDiagnosticProgress != null) PbDiagnosticProgress.Value = (index + 1) * 10;
            if (TxtDiagStep != null) TxtDiagStep.Text = $"Question {index + 1} of 10";

            if (BorderDiagResult != null) BorderDiagResult.Visibility = Visibility.Collapsed;
            if (BtnDiagNext != null) BtnDiagNext.IsEnabled = true;
            if (BtnDiagNext != null) BtnDiagNext.Content = index == questions.Count - 1 ? "Complete Diagnostic 🏁" : "Next Question ➡️";
        }

        private void BtnDiagNext_Click(object sender, RoutedEventArgs e)
        {
            int selectedOpt = -1;
            if (RadDiagOptA.IsChecked == true) selectedOpt = 0;
            else if (RadDiagOptB.IsChecked == true) selectedOpt = 1;
            else if (RadDiagOptC.IsChecked == true) selectedOpt = 2;
            else if (RadDiagOptD.IsChecked == true) selectedOpt = 3;

            if (selectedOpt == -1)
            {
                MessageBox.Show("Please select an answer option to proceed.", "Diagnostic Question", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
                return;
            }

            _diagAnswers[_diagCurrentIndex] = selectedOpt;

            var questions = ZeroToHeroService.GetDiagnosticQuestions();
            if (_diagCurrentIndex < questions.Count - 1)
            {
                _diagCurrentIndex++;
                LoadDiagnosticQuestion(_diagCurrentIndex);
            }
            else
            {
                // Finished
                var (tier, title, summary) = ZeroToHeroService.EvaluateDiagnostic(_diagAnswers);
                if (TxtDiagResultTitle != null) TxtDiagResultTitle.Text = title;
                if (TxtDiagResultTrajectory != null) TxtDiagResultTrajectory.Text = summary;
                if (BorderDiagResult != null) BorderDiagResult.Visibility = Visibility.Visible;
                if (BtnDiagNext != null) BtnDiagNext.IsEnabled = false;
            }
        }

        private void BtnResetDiag_Click(object sender, RoutedEventArgs e)
        {
            _diagCurrentIndex = 0;
            _diagAnswers = new int[10];
            if (BtnDiagNext != null) BtnDiagNext.IsEnabled = true;
            LoadDiagnosticQuestion(0);
        }

        private void BtnAcceptDiagPath_Click(object sender, RoutedEventArgs e)
        {
            SwitchTab("Courses");
        }

        // --- CAPSTONE BUILDER STUDIO ---
        private void InitCapstoneSelector()
        {
            if (CmbHeroCapstones == null || CmbHeroCapstones.Items.Count > 0) return;

            var capstones = ZeroToHeroService.GetAllHeroCapstones();
            foreach (var cap in capstones)
            {
                CmbHeroCapstones.Items.Add(new ComboBoxItem { Content = cap.Title, Tag = cap });
            }
            CmbHeroCapstones.SelectedIndex = 0;
        }

        private void CmbHeroCapstones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbHeroCapstones?.SelectedItem is ComboBoxItem item && item.Tag is HeroCapstoneProject cap)
            {
                _currentSelectedCapstone = cap;
                if (TxtCapstoneTitle != null) TxtCapstoneTitle.Text = cap.Title;
                if (TxtCapstonePillar != null) TxtCapstonePillar.Text = cap.TechPillar;
                if (TxtCapstoneDesc != null) TxtCapstoneDesc.Text = cap.Description;
                if (TxtCapstoneSpecs != null) TxtCapstoneSpecs.Text = cap.ArchitectureSpecs;
                if (TxtCapstoneCode != null) TxtCapstoneCode.Text = cap.StarterCode;
                if (TxtCapstoneOutput != null) TxtCapstoneOutput.Text = "Testbench ready. Click 'Run Testbench & Verify' to execute unit assertions.";
            }
        }

        private void BtnResetCapstoneCode_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectedCapstone != null && TxtCapstoneCode != null)
            {
                TxtCapstoneCode.Text = _currentSelectedCapstone.StarterCode;
                TxtCapstoneOutput.Text = "Starter code restored.";
            }
        }

        private void BtnVerifyCapstone_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectedCapstone == null || TxtCapstoneCode == null || TxtCapstoneOutput == null) return;

            string code = TxtCapstoneCode.Text;
            var result = ZeroToHeroService.EvaluateCapstone(_currentSelectedCapstone.Id, code);

            TxtCapstoneOutput.Text = string.Join(Environment.NewLine, result.TestOutputLogs);

            if (result.Passed)
            {
                _db.SaveHeroCapstoneSubmission(1, _currentSelectedCapstone.Id, result.Score, result.XpEarned, code);
                int newXp = _db.AddUserXp(result.XpEarned);
                TxtUserXp.Text = $"⭐ {newXp} XP";
                UpdateHeroRadarUI();
                MessageBox.Show($"🎉 CONGRATULATIONS!\n\n{result.Summary}\n\nYou earned +{result.XpEarned} XP towards your Hero certification!", "Capstone Verified", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        // --- HERO READINESS RADAR & PORTFOLIO ---
        private void UpdateHeroRadarUI()
        {
            var report = ZeroToHeroService.CalculateHeroReadiness(1, _db);

            if (TxtOverallHeroScore != null) TxtOverallHeroScore.Text = $"{report.OverallPercentage}% HERO READINESS";
            if (TxtHeroTierBadge != null) TxtHeroTierBadge.Text = report.ReadinessTier;

            if (PbRadarCoding != null) PbRadarCoding.Value = report.CodingScore;
            if (TxtRadarCoding != null) TxtRadarCoding.Text = $"{report.CodingScore}%";

            if (PbRadarSystems != null) PbRadarSystems.Value = report.SystemsScore;
            if (TxtRadarSystems != null) TxtRadarSystems.Text = $"{report.SystemsScore}%";

            if (PbRadarNetworks != null) PbRadarNetworks.Value = report.NetworksScore;
            if (TxtRadarNetworks != null) TxtRadarNetworks.Text = $"{report.NetworksScore}%";

            if (PbRadarCyber != null) PbRadarCyber.Value = report.CyberScore;
            if (TxtRadarCyber != null) TxtRadarCyber.Text = $"{report.CyberScore}%";

            if (PbRadarDb != null) PbRadarDb.Value = report.DatabaseScore;
            if (TxtRadarDb != null) TxtRadarDb.Text = $"{report.DatabaseScore}%";

            if (PbRadarAi != null) PbRadarAi.Value = report.AiScore;
            if (TxtRadarAi != null) TxtRadarAi.Text = $"{report.AiScore}%";

            // Update recommendations
            if (StackHeroRecommendations != null)
            {
                StackHeroRecommendations.Children.Clear();
                foreach (var rec in report.RecommendedMilestones)
                {
                    var txt = new TextBlock
                    {
                        Text = $"👉 {rec}",
                        FontSize = 12,
                        Foreground = new SolidColorBrush(Color.FromRgb(0xCB, 0xD5, 0xE1)),
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 6)
                    };
                    StackHeroRecommendations.Children.Add(txt);
                }
            }

            // Update completed capstones
            var completedIds = _db.GetUserCompletedHeroCapstoneIds(1);
            if (TxtCompletedCapstonesSummary != null)
            {
                if (completedIds.Count == 0)
                {
                    TxtCompletedCapstonesSummary.Text = "No capstones completed yet. Build your first system in the Capstone Builder!";
                }
                else
                {
                    var allCaps = ZeroToHeroService.GetAllHeroCapstones();
                    var completedNames = allCaps.Where(c => completedIds.Contains(c.Id)).Select(c => $"✅ {c.Title}");
                    TxtCompletedCapstonesSummary.Text = string.Join(Environment.NewLine, completedNames);
                }
            }
        }

        private void BtnRefreshRadar_Click(object sender, RoutedEventArgs e)
        {
            UpdateHeroRadarUI();
        }

        private void BtnExportHeroPortfolio_Click(object sender, RoutedEventArgs e)
        {
            var report = ZeroToHeroService.CalculateHeroReadiness(1, _db);
            var completedIds = _db.GetUserCompletedHeroCapstoneIds(1);
            var allCaps = ZeroToHeroService.GetAllHeroCapstones();

            var portfolioJson = new
            {
                Student = "Dharmesh Varia",
                Academy = "Bhavani Technology Academy",
                HeroReadinessScore = $"{report.OverallPercentage}%",
                HeroTier = report.ReadinessTier,
                CompetencyRadar = new
                {
                    CodingAndAlgorithms = $"{report.CodingScore}%",
                    SystemsAndHardware = $"{report.SystemsScore}%",
                    NetworksAndCloud = $"{report.NetworksScore}%",
                    Cybersecurity = $"{report.CyberScore}%",
                    RelationalDatabases = $"{report.DatabaseScore}%",
                    ArtificialIntelligence = $"{report.AiScore}%"
                },
                CompletedCapstones = allCaps.Where(c => completedIds.Contains(c.Id)).Select(c => new { c.Id, c.Title, c.TechPillar }).ToList(),
                ExportTimestamp = DateTime.UtcNow.ToString("o")
            };

            string jsonStr = System.Text.Json.JsonSerializer.Serialize(portfolioJson, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            string exportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HeroPortfolioCard.json");
            System.IO.File.WriteAllText(exportPath, jsonStr);

            MessageBox.Show($"📜 Verified Engineer Portfolio Card Exported Successfully!\n\nFile saved to:\n{exportPath}\n\nHero Readiness: {report.OverallPercentage}% ({report.ReadinessTier})", "Portfolio Exported", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
        }

        // =====================================================================
        // ULTIMATE FEATURES & LABS IMPLEMENTATION
        // =====================================================================
        private void AwardUserXp(int xp)
        {
            int totalXp = _db.AddUserXp(xp);
            TxtUserXp.Text = $"⭐ {totalXp} XP";
            LoadUserData();
        }

        // 1. GLOBAL KEYBOARD SHORTCUTS & THEMES
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.D1:
                        NavDashboard.IsChecked = true;
                        Nav_Click(NavDashboard, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.D2:
                        NavLabs.IsChecked = true;
                        Nav_Click(NavLabs, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.D3:
                        NavZeroToHero.IsChecked = true;
                        Nav_Click(NavZeroToHero, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.D4:
                        NavGaming.IsChecked = true;
                        Nav_Click(NavGaming, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.D5:
                    case Key.D6:
                        NavMastery.IsChecked = true;
                        Nav_Click(NavMastery, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.Enter:
                        if (TabsCreativeLabs != null && TabsCreativeLabs.SelectedItem is TabItem t && t.Header.ToString()!.Contains("Debugger"))
                        {
                            BtnDebuggerStep_Click(this, new RoutedEventArgs());
                        }
                        else
                        {
                            BtnRunCode_Click(this, new RoutedEventArgs());
                        }
                        e.Handled = true;
                        break;
                }
            }
            else if (e.Key == Key.F11)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                e.Handled = true;
            }
            else if (e.Key == Key.F10)
            {
                BtnDebuggerStep_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void CmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbTheme?.SelectedItem is ComboBoxItem item)
            {
                string theme = item.Content.ToString() ?? "";
                if (theme.Contains("Daylight"))
                {
                    Background = (Brush)new BrushConverter().ConvertFrom("#F1F5F9")!;
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#0F172A")!;
                    if (TxtCodeInput != null)
                    {
                        TxtCodeInput.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF")!;
                        TxtCodeInput.Foreground = (Brush)new BrushConverter().ConvertFrom("#24292E")!; // GitHub Light
                    }
                }
                else if (theme.Contains("Terminal"))
                {
                    Background = (Brush)new BrushConverter().ConvertFrom("#021A0C")!;
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#4ADE80")!;
                    if (TxtCodeInput != null)
                    {
                        TxtCodeInput.Background = (Brush)new BrushConverter().ConvertFrom("#021A0C")!;
                        TxtCodeInput.Foreground = (Brush)new BrushConverter().ConvertFrom("#4ADE80")!; // Matrix Terminal
                    }
                }
                else // Night (Dracula)
                {
                    Background = (Brush)new BrushConverter().ConvertFrom("#0F172A")!;
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#F8FAFC")!;
                    if (TxtCodeInput != null)
                    {
                        TxtCodeInput.Background = (Brush)new BrushConverter().ConvertFrom("#282A36")!;
                        TxtCodeInput.Foreground = (Brush)new BrushConverter().ConvertFrom("#F8F8F2")!; // Dracula
                    }
                }
            }
        }

        private void BtnSpeakLesson_Click(object sender, RoutedEventArgs e)
        {
            string speech = $"{TxtLessonTitle.Text}. {TxtLessonContent.Text}";
            if (string.IsNullOrWhiteSpace(speech) || speech.Contains("Select a Course"))
            {
                speech = "Please select a lesson from the curriculum explorer to hear the voice mentor narration.";
            }
            TextToSpeechService.SpeakAsync(speech);
        }

        private void BtnStopVoice_Click(object sender, RoutedEventArgs e)
        {
            TextToSpeechService.Stop();
        }

        // 2. VISUAL STEP-DEBUGGER
        private void InitStepDebugger()
        {
            CmbDebuggerSnippet.Items.Clear();
            CmbDebuggerSnippet.Items.Add("Python / C#: Fibonacci Sequence");
            CmbDebuggerSnippet.Items.Add("Call Stack: Payment Processing Flow");
            CmbDebuggerSnippet.Items.Add("Arithmetic: Sum Loop Accumulator");
            CmbDebuggerSnippet.SelectedIndex = 0;
            LoadDebuggerSnippet("fibonacci");
        }

        private void CmbDebuggerSnippet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbDebuggerSnippet == null || LstDebuggerCode == null) return;
            if (CmbDebuggerSnippet.SelectedIndex == 0) LoadDebuggerSnippet("fibonacci");
            else if (CmbDebuggerSnippet.SelectedIndex == 1) LoadDebuggerSnippet("callstack");
            else LoadDebuggerSnippet("sum");
        }

        private void LoadDebuggerSnippet(string snippetType)
        {
            _debuggerFrames = CodeStepDebuggerService.TraceExecution(snippetType);
            _debuggerFrameIndex = 0;
            UpdateDebuggerUI();
        }

        private void UpdateDebuggerUI()
        {
            if (LstDebuggerCode == null || _debuggerFrames == null || _debuggerFrames.Count == 0) return;

            int validIdx = Math.Clamp(_debuggerFrameIndex, 0, _debuggerFrames.Count - 1);
            var frame = _debuggerFrames[validIdx];

            LstDebuggerCode.Items.Clear();
            for (int i = 0; i < _debuggerFrames.Count; i++)
            {
                string prefix = (i == validIdx) ? "👉 " : "   ";
                LstDebuggerCode.Items.Add($"{prefix}Line {_debuggerFrames[i].LineNumber}: {_debuggerFrames[i].CodeLine}");
            }
            LstDebuggerCode.SelectedIndex = validIdx;

            LstDebuggerVariables.Items.Clear();
            foreach (var kvp in frame.Variables)
            {
                LstDebuggerVariables.Items.Add($"  {kvp.Key} = {kvp.Value}");
            }
            if (frame.Variables.Count == 0)
            {
                LstDebuggerVariables.Items.Add("  (No locals in scope)");
            }

            LstDebuggerCallStack.Items.Clear();
            foreach (var s in frame.CallStack)
            {
                LstDebuggerCallStack.Items.Add($"  [Frame] {s}");
            }

            TxtDebuggerConsole.Text = frame.ConsoleOutput;
        }

        private void BtnDebuggerStep_Click(object sender, RoutedEventArgs e)
        {
            if (_debuggerFrameIndex < _debuggerFrames.Count - 1)
            {
                _debuggerFrameIndex++;
                UpdateDebuggerUI();
            }
            else
            {
                MessageBox.Show("Execution reached end of program breakpoint.", "Debugger", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        private void BtnDebuggerRunAll_Click(object sender, RoutedEventArgs e)
        {
            if (_debuggerFrames.Count > 0)
            {
                _debuggerFrameIndex = _debuggerFrames.Count - 1;
                UpdateDebuggerUI();
            }
        }

        private void BtnDebuggerReset_Click(object sender, RoutedEventArgs e)
        {
            _debuggerFrameIndex = 0;
            UpdateDebuggerUI();
        }

        // 3. VISUAL GIT & MERGE STUDIO
        private void InitGitStudio()
        {
            RefreshGitUI();
        }

        private void RefreshGitUI()
        {
            TxtGitCurrentBranch.Text = _gitSim.CurrentBranch;
            CmbGitBranches.Items.Clear();
            foreach (var b in _gitSim.GetBranches())
            {
                CmbGitBranches.Items.Add(b);
            }
            CmbGitBranches.SelectedItem = _gitSim.CurrentBranch;

            LstGitGraph.Items.Clear();
            var commits = _gitSim.GetCommitGraph();
            for (int i = commits.Count - 1; i >= 0; i--)
            {
                var c = commits[i];
                string headTag = (c.BranchName == _gitSim.CurrentBranch && i == commits.Count - 1) ? " [HEAD]" : "";
                LstGitGraph.Items.Add($"* ({c.CommitHash}) - {c.Message} ({c.BranchName}){headTag}");
            }
        }

        private void BtnGitCommit_Click(object sender, RoutedEventArgs e)
        {
            string msg = TxtGitCommitMsg.Text.Trim();
            if (string.IsNullOrEmpty(msg)) msg = "Updated source codebase";
            var commit = _gitSim.Commit(msg);
            TxtGitConsole.Text = $"[commit] [{commit.BranchName} {commit.CommitHash}] {commit.Message}\n 1 file changed, 14 insertions(+)";
            RefreshGitUI();
        }

        private void BtnGitBranch_Click(object sender, RoutedEventArgs e)
        {
            string bName = TxtGitNewBranchName.Text.Trim();
            if (!string.IsNullOrEmpty(bName) && _gitSim.CreateBranch(bName))
            {
                TxtGitConsole.Text = $"[branch] Created new branch '{bName}'.";
                RefreshGitUI();
            }
            else
            {
                TxtGitConsole.Text = $"[branch error] Branch name invalid or already exists.";
            }
        }

        private void BtnGitCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (CmbGitBranches.SelectedItem is string b && _gitSim.Checkout(b))
            {
                TxtGitConsole.Text = $"[checkout] Switched to branch '{b}'.";
                RefreshGitUI();
            }
        }

        private void BtnGitMerge_Click(object sender, RoutedEventArgs e)
        {
            if (CmbGitBranches.SelectedItem is string b)
            {
                var result = _gitSim.Merge(b);
                TxtGitConsole.Text = result.MergeMessage;
                RefreshGitUI();
            }
        }

        private void BtnGitReset_Click(object sender, RoutedEventArgs e)
        {
            _gitSim.Reset();
            TxtGitConsole.Text = "[git init] Repository re-initialized to initial clean commit.";
            RefreshGitUI();
        }

        // 4. OFFLINE REST API CLIENT
        private void InitRestWorkbench()
        {
            TxtRestRequestBody.Text = "{\n  \"name\": \"Dharmesh Varia\",\n  \"track\": \"Full-Stack Systems & AI\"\n}";
        }

        private void CmbRestEndpoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbRestEndpoint == null || TxtRestUrl == null || CmbRestMethod == null || TxtRestRequestBody == null) return;
            if (CmbRestEndpoint.SelectedIndex == 0)
            {
                CmbRestMethod.SelectedIndex = 0;
                TxtRestUrl.Text = "/api/v1/students";
            }
            else if (CmbRestEndpoint.SelectedIndex == 1)
            {
                CmbRestMethod.SelectedIndex = 0;
                TxtRestUrl.Text = "/api/v1/courses";
            }
            else if (CmbRestEndpoint.SelectedIndex == 2)
            {
                CmbRestMethod.SelectedIndex = 1;
                TxtRestUrl.Text = "/api/v1/students";
                TxtRestRequestBody.Text = "{\n  \"username\": \"student42\",\n  \"email\": \"student@bhavanitech.org\"\n}";
            }
            else if (CmbRestEndpoint.SelectedIndex == 3)
            {
                CmbRestMethod.SelectedIndex = 0;
                TxtRestUrl.Text = "/api/v1/leaderboard";
            }
            else if (CmbRestEndpoint.SelectedIndex == 4)
            {
                CmbRestMethod.SelectedIndex = 0;
                TxtRestUrl.Text = "/api/v1/health";
            }
        }

        private void BtnRestSend_Click(object sender, RoutedEventArgs e)
        {
            string method = (CmbRestMethod.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "GET";
            string url = TxtRestUrl.Text.Trim();
            string body = TxtRestRequestBody.Text;

            var req = new RestApiRequest(method, url, new Dictionary<string, string>(), body);
            var res = OfflineRestApiClientService.SendRequest(req);

            TxtRestStatus.Text = $"{res.StatusCode} {res.StatusText}";
            TxtRestStatus.Foreground = res.StatusCode < 300 
                ? (Brush)new BrushConverter().ConvertFrom("#4ADE80")! 
                : (Brush)new BrushConverter().ConvertFrom("#EF4444")!;

            TxtRestLatency.Text = $"⚡ {res.LatencyMs} ms (Local Engine)";
            TxtRestResponseBody.Text = res.Body;
        }

        // 5. REGEX PATTERN STUDIO
        private void InitRegexLab()
        {
            RunRegexEvaluation();
        }

        private void CmbRegexPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbRegexPresets == null || TxtRegexPattern == null || TxtRegexTestInput == null) return;
            switch (CmbRegexPresets.SelectedIndex)
            {
                case 0:
                    TxtRegexPattern.Text = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                    TxtRegexTestInput.Text = "Contact support@bhavanitech.org or admin@school.edu.in";
                    break;
                case 1:
                    TxtRegexPattern.Text = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
                    TxtRegexTestInput.Text = "DNS resolved to 192.168.1.1 and gateway 10.0.0.1";
                    break;
                case 2:
                    TxtRegexPattern.Text = @"#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})\b";
                    TxtRegexTestInput.Text = "Primary: #38BDF8, Secondary: #0F172A, Accent: #FACC15";
                    break;
                case 3:
                    TxtRegexPattern.Text = @"\b\d{4}-\d{2}-\d{2}\b";
                    TxtRegexTestInput.Text = "Created on 2026-09-05 and expires on 2027-01-01";
                    break;
                case 4:
                    TxtRegexPattern.Text = @"https?:\/\/[^\s]+";
                    TxtRegexTestInput.Text = "Visit https://bhavanitech.org/academy for guides";
                    break;
                case 5:
                    TxtRegexPattern.Text = @"\b\d+\b";
                    TxtRegexTestInput.Text = "Scores: 100, 85, 42 and 99";
                    break;
            }
            RunRegexEvaluation();
        }

        private void TxtRegexPattern_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunRegexEvaluation();
        }

        private void TxtRegexTestInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunRegexEvaluation();
        }

        private void RunRegexEvaluation()
        {
            if (TxtRegexPattern == null || TxtRegexTestInput == null || LstRegexMatches == null) return;
            string pattern = TxtRegexPattern.Text;
            string input = TxtRegexTestInput.Text;

            var res = RegexLabService.Evaluate(pattern, input);
            TxtRegexMatchSummary.Text = $"Matches Found: {res.MatchCount}";
            TxtRegexExplanation.Text = res.Explanation;

            LstRegexMatches.Items.Clear();
            foreach (var m in res.Matches)
            {
                LstRegexMatches.Items.Add($"  Match: \"{m}\"");
            }
            if (res.Matches.Count == 0)
            {
                LstRegexMatches.Items.Add("  (No regex matches detected)");
            }
        }

        // 6. CLI CHEAT SHEETS
        private void InitCheatSheets()
        {
            RefreshCheatSheetsList();
        }

        private void CmbCheatCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCheatCategory?.SelectedItem is ComboBoxItem item)
            {
                _activeCheatCategory = item.Content.ToString() ?? "All Categories";
                RefreshCheatSheetsList();
            }
        }

        private void TxtCheatSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtCheatSearch != null && TxtCheatSearch.Text != "Search command or keyword...")
            {
                RefreshCheatSheetsList();
            }
        }

        private void TxtCheatSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtCheatSearch != null && TxtCheatSearch.Text == "Search command or keyword...")
            {
                TxtCheatSearch.Text = "";
            }
        }

        private void RefreshCheatSheetsList()
        {
            if (LstCheatSheets == null) return;
            string? cat = _activeCheatCategory.Contains("All") ? null : _activeCheatCategory.Split(' ')[0];
            string? search = (TxtCheatSearch != null && TxtCheatSearch.Text != "Search command or keyword...") ? TxtCheatSearch.Text.Trim() : null;

            var items = CheatSheetService.GetCheatSheets(cat, search);
            LstCheatSheets.Items.Clear();
            foreach (var item in items)
            {
                LstCheatSheets.Items.Add($"[{item.Category}] {item.Command} — {item.Description}");
            }
            if (items.Count > 0)
            {
                LstCheatSheets.SelectedIndex = 0;
            }
        }

        private void LstCheatSheets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstCheatSheets != null && LstCheatSheets.SelectedIndex >= 0)
            {
                string? cat = _activeCheatCategory.Contains("All") ? null : _activeCheatCategory.Split(' ')[0];
                string? search = (TxtCheatSearch != null && TxtCheatSearch.Text != "Search command or keyword...") ? TxtCheatSearch.Text.Trim() : null;
                var items = CheatSheetService.GetCheatSheets(cat, search);

                if (LstCheatSheets.SelectedIndex < items.Count)
                {
                    var selected = items[LstCheatSheets.SelectedIndex];
                    TxtCheatTitle.Text = selected.Command;
                    TxtCheatMeta.Text = $"Category: {selected.Category} | Description: {selected.Description}";
                    TxtCheatExample.Text = selected.Example;
                    TxtCheatDetail.Text = $"Standard usage in {selected.Category}:\n\nCommand: {selected.Command}\nDescription: {selected.Description}\nExample Syntax: {selected.Example}\n\nTip: Test this command inside the Virtual Terminal or Creative Lab terminal emulator.";
                }
            }
        }

        // 7. ARCADE GAMES: ASSEMBLY, WEBCRAFT & CTF
        private void InitAssemblyArena()
        {
            TxtAssemblyRegisters.Text = "RAX: 0 | RBX: 0 | RCX: 0 | RDX: 0\nZF: 0 | CF: 0 | Drone HP: 100";
        }

        private void BtnRunAssemblyBot_Click(object sender, RoutedEventArgs e)
        {
            string code = TxtAssemblyCode.Text;
            var result = ArcadeGameEngine.ExecuteAssemblyBot(code, 100);

            TxtAssemblyOutput.Text = string.Join(Environment.NewLine, result.Logs);
            TxtAssemblyRegisters.Text = $"Drone HP: {result.DroneHpRemaining}\nStatus: {(result.Victory ? "🏆 VICTORY (Drone Destroyed)" : "⚔️ Engaged in Combat")}";

            if (result.Victory)
            {
                AwardUserXp(60);
                MessageBox.Show("🎉 VICTORY! Micro-Bot executed assembly directives flawlessly and destroyed the malware drone! (+60 XP)", "Assembly Arena", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        private void BtnResetAssemblyBot_Click(object sender, RoutedEventArgs e)
        {
            TxtAssemblyCode.Text = "MOV RAX, 10\nMOV RBX, 20\nADD RAX, RBX\nFIRE";
            TxtAssemblyOutput.Text = "Assembly mission reset. Configure registers and fire.";
            InitAssemblyArena();
        }

        private void InitWebcraftGame()
        {
            TxtWebcraftFeedback.Text = "Awaiting CSS layout submission...";
        }

        private void BtnSubmitWebcraft_Click(object sender, RoutedEventArgs e)
        {
            string css = TxtWebcraftCssInput.Text;
            var result = ArcadeGameEngine.EvaluateWebcraftCss(css, 1);

            TxtWebcraftFeedback.Text = result.Feedback;
            if (result.Success)
            {
                PnlWebcraftItems.HorizontalAlignment = HorizontalAlignment.Center;
                PnlWebcraftItems.VerticalAlignment = VerticalAlignment.Center;
                AwardUserXp(50);
                MessageBox.Show("🚀 RESCUE 1 COMPLETE! The astronaut module was successfully aligned! (+50 XP)", "Webcraft Rescuer", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        private void InitCtfArenaGame()
        {
            _ctfActiveChallengeId = 1;
        }

        private void CmbArcadeCtfChallenges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtArcadeCtfDescription == null || TxtArcadeCtfTarget == null) return;
            _ctfActiveChallengeId = CmbArcadeCtfChallenges.SelectedIndex + 1;

            switch (_ctfActiveChallengeId)
            {
                case 1:
                    TxtArcadeCtfDescription.Text = "Challenge 1: Base64 Decryption & Secret Extraction.\nDecode the Base64 ciphertext below to reveal the secret flag formatted as BHAVANI_CTF_SECURE.";
                    TxtArcadeCtfTarget.Text = "QkhBVkFOSV9DVEZfU0VDVVJF";
                    break;
                case 2:
                    TxtArcadeCtfDescription.Text = "Challenge 2: Memory Buffer Hex Canary Inspection.\nFind the 32-bit hex magic marker located inside the memory buffer dump.";
                    TxtArcadeCtfTarget.Text = "0x7FFE0010: 90 90 90 90 DE AD BE EF 41 41 41 41 [CANARY_PTR]";
                    break;
                case 3:
                    TxtArcadeCtfDescription.Text = "Challenge 3: SQL Injection Vulnerability Patching.\nEnter the defensive coding mechanism used to neutralize ' OR 1=1-- queries.";
                    TxtArcadeCtfTarget.Text = "string sql = \"SELECT * FROM users WHERE user = '\" + input + \"'\";";
                    break;
            }
        }

        private void BtnSubmitArcadeCtfFlag_Click(object sender, RoutedEventArgs e)
        {
            string flag = TxtArcadeCtfFlagInput.Text.Trim();
            var result = ArcadeGameEngine.EvaluateCtfFlag(_ctfActiveChallengeId, flag);

            TxtArcadeCtfFeedback.Text = $"[{DateTime.UtcNow:HH:mm:ss}] {result.Feedback}";
            if (result.Correct)
            {
                AwardUserXp(80);
                MessageBox.Show($"🚩 FLAG CAPTURED!\n\n{result.Feedback}", "CTF Arena", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
        }

        // 8. SPACED REPETITION FLASHCARDS & PARENT AUDIT
        private void InitFlashcardsSRS()
        {
            _dueFlashcards = _db.GetFlashcardsForReview(1);
            if (_dueFlashcards.Count == 0)
            {
                _dueFlashcards = SpacedRepetitionService.GetDefaultFlashcards();
            }
            _currentFlashcardIndex = 0;
            DisplayCurrentFlashcard();
        }

        private void DisplayCurrentFlashcard()
        {
            if (_dueFlashcards == null || _dueFlashcards.Count == 0)
            {
                TxtFlashcardQuestion.Text = "🎉 All flashcards reviewed for today! Great job!";
                TxtCardBoxBadge.Text = "Mastery: 100%";
                BrdCardAnswer.Visibility = Visibility.Collapsed;
                BtnFlipCard.Visibility = Visibility.Collapsed;
                BtnCardAgain.Visibility = Visibility.Collapsed;
                BtnCardGood.Visibility = Visibility.Collapsed;
                return;
            }

            if (_currentFlashcardIndex >= _dueFlashcards.Count)
                _currentFlashcardIndex = 0;

            var card = _dueFlashcards[_currentFlashcardIndex];
            TxtCardCategory.Text = $"Category: {card.Category}";
            TxtCardBoxBadge.Text = $"📦 Box {card.BoxLevel} (Leitner System)";
            TxtFlashcardQuestion.Text = card.FrontPrompt;
            TxtFlashcardAnswer.Text = card.BackAnswer;

            BrdCardAnswer.Visibility = Visibility.Collapsed;
            BtnFlipCard.Visibility = Visibility.Visible;
            BtnCardAgain.Visibility = Visibility.Collapsed;
            BtnCardGood.Visibility = Visibility.Collapsed;

            TxtFlashcardStats.Text = $"Cards Due for Review: {_dueFlashcards.Count}\nCurrent Card: #{_currentFlashcardIndex + 1}\nActive Leitner Box: Box {card.BoxLevel}";
        }

        private void BtnFlipCard_Click(object sender, RoutedEventArgs e)
        {
            BrdCardAnswer.Visibility = Visibility.Visible;
            BtnCardAgain.Visibility = Visibility.Visible;
            BtnCardGood.Visibility = Visibility.Visible;
        }

        private void BtnCardAgain_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFlashcardIndex < _dueFlashcards.Count)
            {
                var card = _dueFlashcards[_currentFlashcardIndex];
                var (newBox, nextReview) = SpacedRepetitionService.DemoteCard();
                _db.UpdateFlashcardProgress(1, card.Id, newBox, nextReview.ToString("o"));
            }
            _currentFlashcardIndex++;
            DisplayCurrentFlashcard();
        }

        private void BtnCardGood_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFlashcardIndex < _dueFlashcards.Count)
            {
                var card = _dueFlashcards[_currentFlashcardIndex];
                var (newBox, nextReview) = SpacedRepetitionService.PromoteCard(card.BoxLevel);
                _db.UpdateFlashcardProgress(1, card.Id, newBox, nextReview.ToString("o"));
                AwardUserXp(15);
            }
            _currentFlashcardIndex++;
            DisplayCurrentFlashcard();
        }

        private void RefreshParentAudit()
        {
            var report = ParentAuditService.GenerateReport(1, _db);
            TxtAuditStudentName.Text = $"Student: {report.StudentName} | Verified Offline Academy Record";
            TxtAuditStudyTime.Text = $"{report.TotalStudyMinutes / 60}h {report.TotalStudyMinutes % 60}m Study Time";
            TxtAuditLessonsCompleted.Text = $"{report.CompletedLessonsCount} / {report.TotalCurriculumLessons} Lessons";
            TxtAuditQuizAccuracy.Text = $"{report.QuizAccuracyPercent:F1}% Verified Passing Rate";
            TxtAuditGradeBadge.Text = $"⭐ {report.HeroReadinessTier}";

            var lines = new List<string>
            {
                "STRENGTHS IDENTIFIED BY MENTOR:",
            };
            lines.AddRange(report.Strengths.Select(s => $"  • {s}"));
            lines.Add("\nFOCUS RECOMMENDATIONS FOR NEXT SESSIONS:");
            lines.AddRange(report.FocusAreas.Select(f => $"  • {f}"));

            TxtAuditRecommendations.Text = string.Join(Environment.NewLine, lines);
        }

        private void BtnRefreshAudit_Click(object sender, RoutedEventArgs e)
        {
            RefreshParentAudit();
            MessageBox.Show("Parent & Teacher audit metrics refreshed directly from SQLite database.", "Audit Refreshed", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
        }

        private void BtnExportAuditReport_Click(object sender, RoutedEventArgs e)
        {
            var report = ParentAuditService.GenerateReport(1, _db);
            string json = System.Text.Json.JsonSerializer.Serialize(report, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            string exportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ParentTeacherAuditReport.json");
            System.IO.File.WriteAllText(exportPath, json);

            MessageBox.Show($"📄 Certified Parent & Teacher Progress Audit Exported Successfully!\n\nSaved to:\n{exportPath}", "Audit Exported", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
        }

        private void BtnBackupDatabase_Click(object sender, RoutedEventArgs e)
        {
            string backupPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"bhavani_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
            bool ok = _db.BackupDatabaseToFile(backupPath);
            if (ok)
            {
                TxtBackupStatus.Text = $"✅ Database snapshot verified and backed up to: {System.IO.Path.GetFileName(backupPath)}";
                MessageBox.Show($"💾 SQLite Database Snapshot Created Successfully!\n\nBackup Path:\n{backupPath}", "Database Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowCertificate(_currentUser?.DisplayName ?? "Student", _currentUser?.DateOfBirth ?? "Unknown", _currentPracticalExam.Title);
            }
            else
            {
                TxtBackupStatus.Text = "⚠️ Database snapshot failed. Ensure disk write permissions.";
            }
        }
    }
}

