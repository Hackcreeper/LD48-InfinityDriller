using Unity.VisualScripting;

namespace Upgrades.Handler
{
    public class DrillLevelUpgradeHandler : IUpgradeHandler
    {
        public string GetId()
        {
            return "drill_level";
        }

        public void Handle(Upgrade upgrade, int level, UpgradeStore store)
        {
            switch (level)
            {
                case 2:
                    store.player.level1Body.SetActive(false);
                    store.player.level2Body.SetActive(true);

                    store.player.speed *= 1.2f;
                    store.player.drillLevel = 2;
                    break;
                
                case 3:
                    store.player.level2Body.SetActive(false);
                    store.player.level3Body.SetActive(true);
                    
                    store.player.speed *= 1.5f;
                    store.player.drillLevel = 3;
                    break;
                
                case 4:
                    store.player.level3Body.SetActive(false);
                    store.player.level4Body.SetActive(true);
                    
                    store.player.speed *= 1.3f;
                    store.player.drillLevel = 4;
                    break;
            }
        }
    }
}