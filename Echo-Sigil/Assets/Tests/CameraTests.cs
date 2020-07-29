using NUnit.Framework;
using UnityEngine;
using mapEditor;

namespace Camera_Tests
{
    class selection
    {
        [Test]
        public void get_Screen_point_returns_z0_bottom_left()
        {
            Camera.main.gameObject.AddComponent<TacticsMovementCamera>().FoucusInputs();
            Vector3 point = TacticsMovementCamera.GetScreenPoint(0, 0);
            Assert.AreEqual(0, point.z);
        }
        [Test]
        public void get_Screen_point_returns_z0_top_right()
        {
            Camera.main.gameObject.AddComponent<TacticsMovementCamera>().FoucusInputs();
            Vector3 point = TacticsMovementCamera.GetScreenPoint(Camera.main.pixelWidth, Camera.main.pixelHeight);
            Assert.AreEqual(0, point.z);
        }
        [Test]
        public void get_Screen_point_returns_z0_middle()
        {
            Camera.main.gameObject.AddComponent<TacticsMovementCamera>().FoucusInputs();
            Vector3 point = TacticsMovementCamera.GetScreenPoint(Camera.main.pixelWidth/2, Camera.main.pixelHeight/2);
            Assert.AreEqual(0, point.z);
        }
        [Test]
        public void unrotated_angle_is_0()
        {
            TacticsMovementCamera tacticsMovementCamera = Camera.main.gameObject.AddComponent<TacticsMovementCamera>();
            tacticsMovementCamera.offsetFromFoucus = 0;
            tacticsMovementCamera.FoucusInputs();
            Assert.AreEqual(0,TacticsMovementCamera.GetAngleBetweenCameraForwardAndVectorForward());

        }
        [Test]
        public void center_screen_starts_as_center_tile()
        {
            Camera.main.gameObject.AddComponent<TacticsMovementCamera>().FoucusInputs();
            Vector3 point = TacticsMovementCamera.GetScreenPoint(Camera.main.pixelWidth/2, Camera.main.pixelHeight/2);
            MapReader.GeneratePhysicalMap(SaveSystem.LoadPallate(Application.dataPath + "/Quests/Tests"), new Map(1, 1));
            Assert.IsNotNull(MapReader.GetTile(MapReader.WorldToGridSpace(point)));
        }
    }
    class rotation
    {
        [Test]
        public void camera_rotates_x()
        {
            Camera.main.gameObject.AddComponent<TacticsMovementCamera>().FoucusInputs();
            Assert.AreNotEqual(0, Camera.main.transform.rotation.eulerAngles.x);
        }
        [Test]
        public void camera_rotates_y()
        {
            Camera.main.gameObject.AddComponent<TacticsMovementCamera>().FoucusInputs();
            Assert.AreNotEqual(0, Camera.main.transform.rotation.eulerAngles.y);
        }
    }
}
