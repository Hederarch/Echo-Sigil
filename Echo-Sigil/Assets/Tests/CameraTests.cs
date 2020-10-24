using NUnit.Framework;
using UnityEngine;
using System;

namespace Camera_Tests
{
    class selection
    {
        [Test]
        public void get_Screen_point_returns_z0_bottom_left()
        {
            Vector3 point = ResetCamera().GetScreenPoint(0, 0);
            Assert.AreEqual(0, point.z);
        }

        public static TacticsMovementCamera ResetCamera()
        {
            GameObject gameObject = new GameObject("TestCam");
            TacticsMovementCamera tacticsMovementCamera = gameObject.AddComponent<TacticsMovementCamera>();
            tacticsMovementCamera.cam = gameObject.AddComponent<Camera>();
            tacticsMovementCamera.offsetFromFoucus = 4;
            tacticsMovementCamera.offsetFromZ0 = 4;
            tacticsMovementCamera.angle = (float)Math.PI;
            tacticsMovementCamera.FoucusInputs();
            return tacticsMovementCamera;
        }

        [Test]
        public void get_Screen_point_returns_z0_top_right()
        {
            Vector3 point = ResetCamera().GetScreenPoint(Camera.main.pixelWidth, Camera.main.pixelHeight);
            Assert.AreEqual(0, point.z);
        }
        [Test]
        public void get_Screen_point_returns_z0_middle()
        {
            Vector3 point = ResetCamera().GetScreenPoint(Camera.main.pixelWidth/2, Camera.main.pixelHeight/2);
            Assert.AreEqual(0, point.z);
        }
        [Test]
        public void unrotated_angle_is_0()
        {
            TacticsMovementCamera tacticsMovementCamera = ResetCamera();
            tacticsMovementCamera.offsetFromFoucus = 0;
            tacticsMovementCamera.FoucusInputs();
            Assert.AreEqual(0, tacticsMovementCamera.GetAngleBetweenCameraForwardAndVectorForward());

        }
        [Test]
        public void center_screen_starts_as_center_tile()
        {
            Vector3 point = ResetCamera().GetScreenPoint(Camera.main.pixelWidth/2, Camera.main.pixelHeight/2);
            MapReader.GeneratePhysicalMap();
            Assert.IsNotNull(MapReader.GetTile(MapReader.WorldToGridSpace(point), 0));
        }
    }
    class rotation
    {
        [Test]
        public void camera_rotates_x()
        {
            Assert.AreNotEqual(0, selection.ResetCamera().transform.rotation.eulerAngles.x);
        }
        [Test]
        public void camera_rotates_y()
        {
            Assert.AreNotEqual(0, selection.ResetCamera().transform.rotation.eulerAngles.y);
        }
    }
}
