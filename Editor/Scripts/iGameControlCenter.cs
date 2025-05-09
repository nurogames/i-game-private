using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor.PackageManager;

namespace iGame.Editor
{

    public class iGameControlCenter : EditorWindow
    {
        // Private variables
        private static bool m_IsLoggedIn = false;

        private static iGameControlCenter m_Window = null;

        private static Dictionary<Button, VisualElement> m_Pages = new Dictionary<Button, VisualElement>();
        private static Dictionary<iGamePackage, Button> m_PackageSelection = new Dictionary<iGamePackage, Button>();

        private static iGameProjects m_Projects = null;
        private static iGamePackages m_Packages = null;

        private static iGameProject m_SelectedProject = null;
        private static iGamePackage m_SelectedPackage = null;

        // Public menu functions
        [MenuItem("i-Game/Control Center", priority = 0)]
        public static void OpenMainWindow()
        {
            m_Window = GetWindow<iGameControlCenter>(true, "i-Game");

            if (m_IsLoggedIn)
            {
                m_Window.minSize = new Vector2(1200, 800);
                m_Window.maxSize = new Vector2(1200, 800);
            }
            else
            {
                m_Window.minSize = new Vector2(400, 500);
                m_Window.maxSize = new Vector2(400, 500);
            }

            m_Window.Show();
            //m_Window.ShowAuxWindow();
        }

        [MenuItem("i-Game/Logout")]
        public static void Logout()
        {
            m_IsLoggedIn = false;
        }

        // Public functions
        public void CreateGUI()
        {
            rootVisualElement.Add(InstantiateDocument());
        }

        // Private functions
        private VisualElement InstantiateDocument()
        {
            m_Projects = AssetDatabase.LoadAssetAtPath<iGameProjects>("Packages/com.nuro.i-game.core/Editor/Data/iGameProjects.asset");

            m_Packages = AssetDatabase.LoadAssetAtPath<iGamePackages>("Packages/com.nuro.i-game.core/Editor/Data/iGamePackages.asset");

            VisualTreeAsset documentVisualTreeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.nuro.i-game.core/Editor/UI/iGameControlCenterDocument.uxml");

            VisualElement visualElement = documentVisualTreeAsset.Instantiate();

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.nuro.i-game.core/Editor/UI/iGameStyleSheet.uss");
            visualElement[0].styleSheets.Add(styleSheet);

            SetupLogin(visualElement.Q("LoginPage"));
            SetupMainPage(visualElement.Q("MainPage"));

            return visualElement[0];
        }

        private void SetupLogin(VisualElement loginPage)
        {
            loginPage.style.display = m_IsLoggedIn ? DisplayStyle.None : DisplayStyle.Flex;

            TextField emailField = loginPage.Q<TextField>("LoginEmailTextField");
            TextField passwordField = loginPage.Q<TextField>("LoginPasswordTextField");
            Button loginButton = loginPage.Q<Button>("LoginButton");
            loginButton.clicked += () =>
            {
                string email = emailField.value;
                string password = passwordField.value;
                // Perform login logic here
                Debug.Log($"Email: {email}, Password: {password}");
                m_IsLoggedIn = true;

                // Switch to main page after login
                loginPage.style.display = DisplayStyle.None;
                loginPage.parent.Q("MainPage").style.display = DisplayStyle.Flex;

                m_Window.minSize = new Vector2(1200, 800);
                m_Window.maxSize = new Vector2(1200, 800);
            };
        }

        private void SetupMainPage(VisualElement mainPage)
        {
            mainPage.style.display = m_IsLoggedIn ? DisplayStyle.Flex : DisplayStyle.None;
            SetupMenu(mainPage);
            SetupGeneralPage(mainPage.Q("GeneralPage"));
            SetupProjectsPage(mainPage.Q("ProjectsPage"), mainPage);
            SetupSelectedProjectPage(mainPage.Q("SelectedProjectPage"));
            SetupPackageManagerPage(mainPage.Q("PackageManagerPage"));
            SetupSettingsPage(mainPage.Q("SettingsPage"));
            SetupTextWindow(mainPage.Q("TextWindow"));
        }

        private void SetupMenu(VisualElement menu)
        {
            Button button = menu.Q<Button>("i-GameMenuButton");
            button.clicked += () => { Application.OpenURL("https://igameproject.eu/"); };

            m_Pages.Clear();

            m_Pages.Add(menu.Q<Button>("GeneralMenuButton"), menu.Q("GeneralPage"));
            m_Pages.Add(menu.Q<Button>("ProjectsMenuButton"), menu.Q("ProjectsPage"));
            m_Pages.Add(menu.Q<Button>("SelectedProjectOverviewMenuButton"), menu.Q("SelectedProjectPage"));
            m_Pages.Add(menu.Q<Button>("PackageManagerMenuButton"), menu.Q("PackageManagerPage"));
            m_Pages.Add(menu.Q<Button>("SettingsMenuButton"), menu.Q("SettingsPage"));

            VisualElement selectedProjectMenuContainer = menu.Q("SelectedProjectMenu");
            selectedProjectMenuContainer.style.display = DisplayStyle.None;

            foreach (var page in m_Pages)
            {
                page.Value.style.display = DisplayStyle.None;
                page.Key.clicked += () =>
                {
                    OpenMenu(page.Key);
                };
            }

            OpenMenu(m_Pages.Keys.FirstOrDefault());    
        }

        private void SetupGeneralPage(VisualElement generalPage)
        {
            generalPage.style.display = DisplayStyle.Flex;

            Button aboutButton = generalPage.Q<Button>("AboutIGameButton");
            aboutButton.clicked += () => DisplayTextWindow(IGameUtils.GetTextFromFile("AboutIGame"));

            Button licenseButton = generalPage.Q<Button>("LicenseButton");
            licenseButton.clicked += () => DisplayTextWindow(IGameUtils.GetTextFromFile("License"));

            Button aiDisclaimerButton = generalPage.Q<Button>("AiDisclaimerButton");
            aiDisclaimerButton.clicked += () => DisplayTextWindow(IGameUtils.GetTextFromFile("AiDisclaimer"));
        }

        private void SetupProjectsPage(VisualElement projectsPage, VisualElement mainPage)
        {
            ScrollView scrollView = projectsPage.Q<ScrollView>("ProjectsScrollView");
            scrollView.Clear();
            foreach (var project in m_Projects.Projects)
            {
                Button projectButton = new Button();
                projectButton.AddToClassList("project-button");
                projectButton.clicked += () =>
                {
                    ChangeSelectedProject(project, mainPage);
                };
                scrollView.Add(projectButton);

                Image image = new Image();
                projectButton.Add(image);
                image.image = project.Icon;
                image.AddToClassList("project-button-icon");

                VisualElement infoContainer = new VisualElement();
                projectButton.Add(infoContainer);

                VisualElement titleContainer = new VisualElement();
                titleContainer.style.flexDirection = FlexDirection.Row;
                infoContainer.Add(titleContainer);

                Label nameLabel = new Label();
                titleContainer.Add(nameLabel);
                nameLabel.text = project.Name;
                nameLabel.AddToClassList("project-button-title-label");

                Label creationDateLabel = new Label();
                titleContainer.Add(creationDateLabel);
                creationDateLabel.text = project.CreationDatum;
                creationDateLabel.AddToClassList("project-button-date-label");

                Label descriptionLabel = new Label();
                infoContainer.Add(descriptionLabel);
                descriptionLabel.text = project.Description;
                descriptionLabel.AddToClassList("project-button-label");

            }
        }

        private void SetupSelectedProjectPage(VisualElement selectedProjectPage)
        {

        }

        private void SetupPackageManagerPage(VisualElement packageManagerPage)
        {
            m_PackageSelection.Clear();

            VisualElement packageInfoArea = packageManagerPage.Q("PackageInfoArea");

            ScrollView scrollView = packageManagerPage.Q<ScrollView>("ScrollView");
            scrollView.Clear();
            foreach (var package in m_Packages.Packages)
            {
                Button packageButton = new Button();
                scrollView.Add(packageButton);
                packageButton.AddToClassList("package-manager-selection-button");
                packageButton.text = package.Name;
                packageButton.clicked += () =>
                {
                    SelectPackage(package, packageInfoArea);
                };

                m_PackageSelection.Add(package, packageButton);
            }

            SelectPackage(m_Packages.Packages.FirstOrDefault(), packageInfoArea);


            Button installButton = packageInfoArea.Q<Button>("InstallPackageButton");
            installButton.clicked += () => InstallPackage(packageInfoArea);

            Button removeButton = packageInfoArea.Q<Button>("RemovePackageButton");
            removeButton.clicked += () => RemovePackage(packageInfoArea);
        }

        private void SetupSettingsPage(VisualElement settingsPage)
        {

        }

        private void SetupTextWindow(VisualElement textWindow)
        {
            textWindow.style.display = DisplayStyle.None;
            Button closeButton = textWindow.Q<Button>("TextWindowCloseButton");
            closeButton.clicked += () =>
            {
                textWindow.style.display = DisplayStyle.None;
            };

        }

        private void OpenMenu(Button menuButton)
        {
            if (!m_Pages.ContainsKey(menuButton))
                return;

            foreach (var page in m_Pages)
            {
                if (page.Key == menuButton)
                {
                    page.Key.enabledSelf = false;
                    page.Value.style.display = DisplayStyle.Flex;
                }
                else
                {
                    page.Key.enabledSelf = true;
                    page.Value.style.display = DisplayStyle.None;
                }
            }
        }

        private void ChangeSelectedProject(iGameProject project, VisualElement mainPage)
        {
            m_SelectedProject = project;

            VisualElement selectedProjectMenuContainer = mainPage.Q("SelectedProjectMenu");
            selectedProjectMenuContainer.style.display = DisplayStyle.Flex;

            Label nameLabel = selectedProjectMenuContainer.Q<Label>("SelectedProjectNameLabel");
            nameLabel.text = project.Name;

            Button overviewButton = mainPage.Q<Button>("SelectedProjectOverviewMenuButton");
            OpenMenu(overviewButton);
        }

        private void DisplayTextWindow(string text)
        {
            VisualElement textWindow = rootVisualElement.Q("TextWindow");
            textWindow.style.display = DisplayStyle.Flex;
            rootVisualElement.Q<Label>("TextWindowLabel").text = text;
        }

        private void SelectPackage(iGamePackage package, VisualElement packageInfoArea)
        {
            if (!m_PackageSelection.ContainsKey(package))
                return;

            foreach (var pack in m_PackageSelection)
            {
                pack.Value.enabledSelf = pack.Key != package;
            }

            VisualElement icon = packageInfoArea.Q("PackageIcon");
            if(package.Icon != null)
                icon.style.backgroundImage = package.Icon;

            Label nameLabel = packageInfoArea.Q<Label>("PackageNameLabel");
            nameLabel.text = package.Name;

            Label packageHandleLabel = packageInfoArea.Q<Label>("PackageHandleLabel");
            packageHandleLabel.text = string.IsNullOrEmpty(package.PackageHandle) ? "<i>unknown</i>" : package.PackageHandle;
            packageHandleLabel.selection.isSelectable = true;

            Label gitUrlLabel = packageInfoArea.Q<Label>("PackageGitUrlLabel");
            gitUrlLabel.text = string.IsNullOrEmpty(package.GitUrl) ? "<i>unknown</i>" : package.GitUrl;
            gitUrlLabel.selection.isSelectable = true;

            Label descriptionLabel = packageInfoArea.Q<Label>("PackageDescriptionLabel");
            descriptionLabel.text = package.Description;

            bool isInstalled = string.IsNullOrEmpty(package.GitUrl) ? false : IsPackageInstalled(package);
            m_SelectedPackage = package;

            Button installButton = packageInfoArea.Q<Button>("InstallPackageButton");
            installButton.style.display = isInstalled ? DisplayStyle.None : DisplayStyle.Flex;
            installButton.enabledSelf = !string.IsNullOrEmpty(package.GitUrl);

            Button removeButton = packageInfoArea.Q<Button>("RemovePackageButton");
            removeButton.style.display = isInstalled ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void InstallPackage(VisualElement packageInfoArea)
        {
            Debug.Log($"Installing package: {m_SelectedPackage.Name}");
            
            var request = Client.Add(m_SelectedPackage.GitUrl);

            EditorApplication.update += () =>
            {
                if (request.IsCompleted)
                {
                    EditorApplication.update -= null;
                    SelectPackage(m_SelectedPackage, packageInfoArea);
                }
            };

            SelectPackage(m_SelectedPackage, packageInfoArea);
        }

        private void RemovePackage(VisualElement packageInfoArea)
        {
            Debug.Log($"Removing package: {m_SelectedPackage.Name}");

            var request = Client.Remove(m_SelectedPackage.PackageHandle);

            EditorApplication.update += () =>
            {
                if (request.IsCompleted)
                {
                    EditorApplication.update -= null;
                    SelectPackage(m_SelectedPackage, packageInfoArea);
                }
            };

            SelectPackage(m_SelectedPackage, packageInfoArea);
        }

        private bool IsPackageInstalled(iGamePackage package)
        {
            if (package == null || string.IsNullOrEmpty(package.GitUrl))
                return false;

            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
            {

            }

            if (listRequest.Status == StatusCode.Success)
                return listRequest.Result.Any(p => p.packageId.Contains(package.GitUrl));

            return false;
        }
    }
}