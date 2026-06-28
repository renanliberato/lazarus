using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Lazarus.Tests
{
    /// <summary>
    /// Behavior-first tests that verify documentation matches the passive inference model.
    /// These tests ensure README and VERIFICATION docs correctly describe the passive-watcher approach.
    /// </summary>
    public class DocumentationTests
    {
        private string _readmePath;
        private string _readmeContent;
        private string _verificationPath;
        private string _verificationContent;

        [SetUp]
        public void Setup()
        {
            var projectRoot = Directory.GetCurrentDirectory();
            _readmePath = Path.Combine(projectRoot, "README.md");
            _verificationPath = Path.Combine(projectRoot, "VERIFICATION.md");
            _readmeContent = File.ReadAllText(_readmePath);
            _verificationContent = File.ReadAllText(_verificationPath);
        }

        [Test]
        public void README_Shows_Installing_UIViewWatcher_Without_Calling_Marker_APIs()
        {
            // Assert: README should show simple UIViewWatcher installation without DismissalMarker
            Assert.IsTrue(_readmeContent.Contains("UIViewWatcher"),
                "README should mention UIViewWatcher");

            // Verify the usage example doesn't involve DismissalMarker
            var lines = _readmeContent.Split('\n').Select(l => l.Trim()).ToArray();
            var usageSectionIndex = Array.IndexOf(lines, "## Usage");

            if (usageSectionIndex >= 0)
            {
                // Get lines after "## Usage" until next section
                var usageLines = lines.Skip(usageSectionIndex).TakeWhile((l, i) => i == 0 || !l.StartsWith("## ")).ToArray();
                var usageText = string.Join("\n", usageLines);

                // Should NOT contain DismissalMarker examples in the main usage flow
                Assert.IsFalse(usageText.Contains("DismissalMarker"),
                    "README usage section should not show DismissalMarker as the primary integration path");
            }

            // Should show simple AddComponent example
            Assert.IsTrue(_readmeContent.Contains("AddComponent<UIViewWatcher>()") ||
                         _readmeContent.Contains("AddComponent (UIViewWatcher)"),
                "README should show adding UIViewWatcher component");
        }

        [Test]
        public void README_Explains_Scale_And_Alpha_Inference_Behavior()
        {
            // Assert: README should explain the passive inference behavior
            Assert.IsTrue(_readmeContent.Contains("infer") ||
                         _readmeContent.Contains("observe") ||
                         _readmeContent.Contains("passive"),
                "README should explain that Lazarus infers dismissals through observation");

            // Should mention scale and alpha/CanvasGroup
            var hasScale = _readmeContent.Contains("scale");
            var hasAlpha = _readmeContent.Contains("alpha") || _readmeContent.Contains("CanvasGroup");
            Assert.IsTrue(hasScale && hasAlpha,
                "README should explain both scale and alpha/CanvasGroup inference behavior");
        }

        [Test]
        public void README_Does_Not_Promote_Manual_Dismissal_Intent_Setting()
        {
            // Assert: README should not ask users to set dismiss intent manually
            Assert.IsFalse(_readmeContent.Contains("MarkDismissalPlanned"),
                "README should not instruct users to call MarkDismissalPlanned");

            Assert.IsFalse(_readmeContent.Contains("DismissalIntent"),
                "README should not instruct users to use DismissalIntent");

            // The old manual marker section should be removed or clearly marked as deprecated
            var hasDismissalMarkerSection = _readmeContent.Contains("DismissalMarker") &&
                                           (!_readmeContent.Contains("deprecated") &&
                                            !_readmeContent.Contains("obsolete") &&
                                            !_readmeContent.Contains("legacy"));
            Assert.IsFalse(hasDismissalMarkerSection,
                "DismissalMarker should only appear with deprecation notices");
        }

        [Test]
        public void VERIFICATION_Docs_List_Passive_Inference_Test_Cases()
        {
            // Assert: VERIFICATION should list test cases for passive inference
            var hasAnimationHistoryTests = _verificationContent.Contains("AnimationHistoryTests");
            var hasDefaultDismissalPolicyTests = _verificationContent.Contains("DefaultDismissalPolicyTests");
            var hasUIViewWatcherTests = _verificationContent.Contains("UIViewWatcherTests");

            Assert.IsTrue(hasAnimationHistoryTests,
                "VERIFICATION should list AnimationHistoryTests for passive inference");
            Assert.IsTrue(hasDefaultDismissalPolicyTests,
                "VERIFICATION should list DefaultDismissalPolicyTests for inference validation");
            Assert.IsTrue(hasUIViewWatcherTests,
                "VERIFICATION should list UIViewWatcherTests for passive watcher behavior");

            // Should mention observed animation evidence
            var mentionsObservedEvidence = _verificationContent.Contains("observed") ||
                                         _verificationContent.Contains("passive") ||
                                         _verificationContent.Contains("inference");
            Assert.IsTrue(mentionsObservedEvidence,
                "VERIFICATION should mention observed animation evidence or passive inference");
        }

        [Test]
        public void Tests_Do_Not_Promote_DismissalMarker_As_Normal_Integration_Path()
        {
            // Assert: Main test files should not promote DismissalMarker as the normal path
            var testDir = Path.Combine(Directory.GetCurrentDirectory(), "Tests", "Editor");
            var mainTestFiles = new[]
            {
                "UIViewWatcherTests.cs",
                "AnimationHistoryTests.cs",
                "DefaultDismissalPolicyTests.cs",
                "CustomFilterTests.cs"
            };

            foreach (var testFile in mainTestFiles)
            {
                var testPath = Path.Combine(testDir, testFile);
                if (File.Exists(testPath))
                {
                    var testContent = File.ReadAllText(testPath);

                    // Main tests should not use DismissalMarker
                    // They should use AnimationHistoryObserver and passive inference instead
                    var usesDismissalMarker = testContent.Contains("DismissalMarker") &&
                                             !testFile.Contains("SmokeTest");

                    Assert.IsFalse(usesDismissalMarker,
                        $"{testFile} should not promote DismissalMarker as the normal integration path");

                    // Should use AnimationHistoryObserver
                    var usesObserver = testContent.Contains("AnimationHistoryObserver") ||
                                     testFile.Contains("PackageStructureTests"); // PackageStructureTests is structural
                    Assert.IsTrue(usesObserver,
                        $"{testFile} should use AnimationHistoryObserver for passive inference");
                }
            }
        }

        [Test]
        public void Deprecated_Marker_API_Documented_As_Legacy_Only()
        {
            // Assert: If DismissalMarker appears in docs, it should be marked as deprecated/legacy
            if (_readmeContent.Contains("DismissalMarker"))
            {
                // Should have deprecation warnings nearby
                var lowerReadme = _readmeContent.ToLower();
                Assert.IsTrue(lowerReadme.Contains("deprecated") ||
                             lowerReadme.Contains("obsolete") ||
                             lowerReadme.Contains("legacy") ||
                             lowerReadme.Contains("backwards compatibility"),
                    "DismissalMarker mentions should include deprecation/legacy warnings");
            }

            if (_verificationContent.Contains("DismissalMarker"))
            {
                var lowerVerification = _verificationContent.ToLower();
                Assert.IsTrue(lowerVerification.Contains("deprecated") ||
                             lowerVerification.Contains("obsolete") ||
                             lowerVerification.Contains("legacy") ||
                             lowerVerification.Contains("backwards compatibility"),
                    "DismissalMarker mentions should include deprecation/legacy warnings");
            }

            // DismissalMarkerSmokeTest is explicitly for testing the deprecated API
            var smokeTestPath = Path.Combine(Directory.GetCurrentDirectory(), "Tests", "Editor", "DismissalMarkerSmokeTest.cs");
            if (File.Exists(smokeTestPath))
            {
                var smokeTestContent = File.ReadAllText(smokeTestPath);
                var lowerSmokeTest = smokeTestContent.ToLower();

                // Should clearly state it's for deprecated API
                Assert.IsTrue(lowerSmokeTest.Contains("deprecated") ||
                             lowerSmokeTest.Contains("obsolete") ||
                             lowerSmokeTest.Contains("backwards compatibility") ||
                             lowerSmokeTest.Contains("smoke test"),
                    "DismissalMarkerSmokeTest should clearly indicate it tests the deprecated API");
            }
        }
    }
}
