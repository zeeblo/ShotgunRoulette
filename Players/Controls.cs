using UnityEngine;
using UnityEngine.InputSystem;

namespace ShotgunRoulette.Players
{
    internal class Controls
    {

        private static InputActionAsset actionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
        public static InputActionMap userControls = actionAsset.AddActionMap("UserControls");
        public static InputActionReference? rouletteRef;

        public static void InitControls()
        {
            if (Plugin.rouletteBind == null)
            {
                return;
            }

            InputAction roulette = userControls.AddAction("roulette", InputActionType.Button, binding: "<Keyboard>/" + Plugin.rouletteBind.Value);
            roulette.performed += Roulette_performed;
            userControls.Enable();

            rouletteRef = InputActionReference.Create(roulette);
        }


        private static void Roulette_performed(InputAction.CallbackContext context)
        {
            if (GameNetworkManager.Instance.localPlayerController != null)
            {
                Plugin.ToggleRoulette(GameNetworkManager.Instance.localPlayerController);
            }
            
        }


    }
}
