using HarmonyLib;
using UnityEngine;

namespace YuEzTools.Patches;

// Thanks: https://github.com/SubmergedAmongUs/Submerged/blob/4a5a6b47cbed526670ae4b7eae76acd7c42e35de/Submerged/UI/Patches/MapSelectButtonPatches.cs#L49
class CreateOptionsPickerPatch
{
    [HarmonyPatch(typeof(GameOptionsMapPicker))]
    public static class GameOptionsMapPickerPatch
    {
        [HarmonyPatch(nameof(GameOptionsMapPicker.FixedUpdate))]
        [HarmonyPrefix]
        public static bool Prefix_FixedUpdate(GameOptionsMapPicker __instance)
        {
            if (__instance == null) return true;

            if (__instance.selectedMapId == 3)
                return false;

            return true;
        }
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Awake))]
    class MenuMapPickerPatch
    {
        public static void Postfix(CreateOptionsPicker __instance)
        {
            Transform mapPickerTransform = __instance.transform.Find("MapPicker");
            MapPickerMenu mapPickerMenu = mapPickerTransform.Find("Map Picker Menu").GetComponent<MapPickerMenu>();

            MapFilterButton airhipIconInMenu = __instance.MapMenu.MapButtons[3];
            MapFilterButton fungleIconInMenu = __instance.MapMenu.MapButtons[4];
            MapFilterButton skeldIconInMenu = __instance.MapMenu.MapButtons[0];

            Transform skeldMenuButton = mapPickerMenu.transform.Find("Skeld");
            Transform polusMenuButton = mapPickerMenu.transform.Find("Polus");
            Transform airshipMenuButton = mapPickerMenu.transform.Find("Airship");
            Transform fungleMenuButton = mapPickerMenu.transform.Find("Fungle");

            float xPos = -1f;
            for (int index = 0; index < 6; ++index)
            {
                __instance.MapMenu.MapButtons[index].transform.SetLocalX(xPos);
                xPos += 0.34f;
            }

            if (__instance.mode == SettingsMode.Host)
            {
                mapPickerMenu.transform.localPosition = new Vector3(mapPickerMenu.transform.localPosition.x, 0.85f, mapPickerMenu.transform.localPosition.z);

                mapPickerTransform.localScale = new Vector3(0.86f, 0.85f, 1f);
                mapPickerTransform.transform.localPosition = new Vector3(mapPickerTransform.transform.localPosition.x + 0.05f, mapPickerTransform.transform.localPosition.y + 0.03f, mapPickerTransform.transform.localPosition.z);
            }

            SwapIconOrButtomsPositions(fungleIconInMenu, airhipIconInMenu);

            // set flipped dleks map Icon/button
            __instance.MapMenu.MapButtons[5].SetFlipped(true);

            mapPickerMenu.transform.Find("Backdrop").localScale *= 5;
        }
        private static void SwapIconOrButtomsPositions(Component one, Component two)
        {
            Transform transform1 = one.transform;
            Transform transform2 = two.transform;
            Vector3 position1 = two.transform.position;
            Vector3 position2 = one.transform.position;
            transform1.position = position1;
            Vector3 vector3 = position2;
            transform2.position = vector3;
        }
    }
}