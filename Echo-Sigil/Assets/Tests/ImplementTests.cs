using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Implement_Tests
{
    class battle
    {
        [Test]
        public void health_percent_returns_correctly()
        {
            GameObject gam = new GameObject();
            Unit imp = gam.AddComponent<Unit>();
            JRPGBattle jrpg = gam.AddComponent<JRPGBattle>();
            imp.battle = jrpg;
            jrpg.health = 3;
            jrpg.maxHealth = 5;
            Assert.AreEqual(3f / 5f, imp.battle.HealthPercent);
        }
        [Test]
        public void will_percent_returns_correctly()
        {
            GameObject gam = new GameObject();
            Unit imp = gam.AddComponent<Unit>();
            JRPGBattle jrpg = gam.AddComponent<JRPGBattle>();
            imp.battle = jrpg;
            jrpg.will = 3;
            jrpg.maxWill = 5;
            Assert.AreEqual(3f / 5f, imp.battle.WillPercent);
        }

    }
    class animations
    {
    }
}
