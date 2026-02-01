using UnityEngine;
using YuEzTools.Modules;

namespace YuEzTools.Patches;

public class CreditsPatches
{
    // Change From TONX
    [HarmonyPatch(typeof(CreditsScreenPopUp))]
    internal class CreditsScreenPopUpPatch
    {
        [HarmonyPatch(nameof(CreditsScreenPopUp.OnEnable))]
        public static void Postfix(CreditsScreenPopUp __instance)
        {
            __instance.BackButton.transform.parent.FindChild("Background").gameObject.SetActive(false);
            
            var followUs = __instance.BackButton.transform.parent.FindChild("FollowUs");
            followUs.FindChild("TwitterIcon").gameObject.SetActive(false);
            
            var qqIcon = followUs.FindChild("FacebookIcon");
            qqIcon.GetComponent<TwitterLink>().LinkUrl = Main.QQUrl;
            qqIcon.GetComponent<SpriteRenderer>().sprite = LoadSprite("YuEzTools.Resources.qqlogo.png",2200f);
            
            var dcIcon = followUs.FindChild("Discord-Logo-Color");
            dcIcon.GetComponent<TwitterLink>().LinkUrl = Main.DcUrl;
        }
    }
    [HarmonyPatch(typeof(CreditsController))]
    public class CreditsControllerPatch
    {
        private static List<CreditsController.CreditStruct> GetModCredits()
        {
            // 开发
            var devList = new List<string>();
            // 翻译
            var translatorList = new List<string>();
            // 帮手
            var helperList = new List<string>();
            // 特别帮助者
            var acList = new List<string>();
            foreach (var dev in DevManager.DevUserList)
            {
                string newone = $"<color={dev.Color}>{dev.Name}</color> - <size=60%>{dev.GetDevJob()}</size>";
                if (DevManager.DevJobs.Any(f => dev.Jobs.Contains(f)))
                    devList.Add(newone); 
                else if(DevManager.TransJobs.Any(f => dev.Jobs.Contains(f)))
                    translatorList.Add(newone); 
                else if(DevManager.HelperJobs.Any(f => dev.Jobs.Contains(f)))
                    helperList.Add(newone); 
                else if(DevManager.SpecialJobs.Any(f => dev.Jobs.Contains(f)))
                    acList.Add(newone); 
                    
            }

            var credits = new List<CreditsController.CreditStruct>();

            AddTitleToCredits(ColorString(Main.ModColor32, Main.ModName));
            
            if(devList.Count != 0)
            {
                AddTitleToCredits(GetString("Developer"));
                AddPersonToCredits(devList);
                AddSpaceToCredits();
            }

            if (translatorList.Count != 0)
            {
                AddTitleToCredits(GetString("Translator"));
                AddPersonToCredits(translatorList);
                AddSpaceToCredits();
            }

            if (helperList.Count != 0)
            {
                AddTitleToCredits(GetString("Contributor"));
                AddPersonToCredits(helperList);
                AddSpaceToCredits();
            }

            if (acList.Count != 0)
            {
                AddTitleToCredits(GetString("Acknowledgement"));
                AddPersonToCredits(acList);
                AddSpaceToCredits();
            }

            return credits;

            void AddSpaceToCredits()
            {
                AddTitleToCredits(string.Empty);
            }
            void AddTitleToCredits(string title)
            {
                credits.Add(new()
                {
                    format = "title",
                    columns = new[] { title },
                });
            }
            void AddPersonToCredits(List<string> list)
            {
                foreach (var line in list)
                {
                    var cols = line.Split(" - ").ToList();
                    if (cols.Count < 2) cols.Add(string.Empty);
                    credits.Add(new()
                    {
                        format = "person",
                        columns = cols.ToArray(),
                    });
                }
            }
        }

        [HarmonyPatch(nameof(CreditsController.AddCredit)), HarmonyPrefix]
        public static void AddCreditPrefix(CreditsController __instance, [HarmonyArgument(0)] CreditsController.CreditStruct originalCredit)
        {
            if (originalCredit.columns[0] != "logoImage") return;

            foreach (var credit in GetModCredits())
            {
                __instance.AddCredit(credit);
                __instance.AddFormat(credit.format);
            }
        }
    }
}