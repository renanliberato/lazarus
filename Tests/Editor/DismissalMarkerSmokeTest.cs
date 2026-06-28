using NUnit.Framework;
using UnityEngine;
using Lazarus;

namespace Lazarus.Tests
{
    /// <summary>
    /// Behavior-level smoke test for the deprecated DismissalMarker.
    /// This test verifies that the deprecated API still exists for backwards compatibility
    /// but clearly signals obsolescence through Obsolete attributes.
    /// </summary>
    public class DismissalMarkerSmokeTest
    {
        [Test]
        public void DismissalMarker_Can_Be_Created_On_GameObject()
        {
            // Arrange: Create a GameObject with the DismissalMarker component
            var gameObject = new GameObject("TestView");
            var marker = gameObject.AddComponent<DismissalMarker>();

            // Assert: Verify the marker exists and implements the public interface
            Assert.IsNotNull(marker, "DismissalMarker component should be added successfully");
            Assert.IsInstanceOf<IDismissalMarker>(marker, "DismissalMarker should implement IDismissalMarker");
        }

        [Test]
        public void DismissalMarker_MarkDismissalPlanned_Updates_State()
        {
            // Arrange: Create a GameObject with DismissalMarker
            var gameObject = new GameObject("TestView");
            var marker = gameObject.AddComponent<DismissalMarker>();

            // Act: Mark dismissal as planned through public interface
            marker.MarkDismissalPlanned();

            // Assert: Verify the state changed correctly
            var dismissalMarker = (DismissalMarker)marker;
            Assert.IsTrue(dismissalMarker.IsDismissalPlanned(), "Dismissal should be marked as planned");
        }

        [Test]
        public void DismissalMarker_Default_State_Is_Not_Planned()
        {
            // Arrange: Create a GameObject with DismissalMarker
            var gameObject = new GameObject("TestView");
            var marker = gameObject.AddComponent<DismissalMarker>();

            // Assert: Verify default state
            Assert.IsFalse(marker.IsDismissalPlanned(), "Default state should be dismissal not planned");
        }

        [Test]
        public void IDismissalMarker_Interface_Provides_IsDismissalPlanned_Method()
        {
            // Arrange: Create a GameObject with DismissalMarker
            var gameObject = new GameObject("TestView");
            var marker = gameObject.AddComponent<DismissalMarker>();
            var interfaceMarker = (IDismissalMarker)marker;

            // Act & Assert: Verify the interface provides the method without casting
            Assert.IsFalse(interfaceMarker.IsDismissalPlanned(), "Interface should provide IsDismissalPlanned method");

            interfaceMarker.MarkDismissalPlanned();
            Assert.IsTrue(interfaceMarker.IsDismissalPlanned(), "Interface method should work correctly");
        }
    }
}