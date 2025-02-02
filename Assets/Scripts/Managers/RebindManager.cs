using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindManager : MonoBehaviour
{
    private InputActionRebindingExtensions.RebindingOperation m_Rebind;

    public void ChangeBinding(int index)
    {
        m_Rebind.OnApplyBinding(nameof(Rebind));
    }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {
    }

    private void Rebind(InputActionRebindingExtensions.RebindingOperation rebind, string t)
    {
        List<InputRebinding> rebindings = InputProfileLoader.GetProfile(CharacterSelectionClass.SelectedProfiles[i]);
        if (rebindings == null)
            continue;

        foreach (InputRebinding rebinding in rebindings)
        {
            InputAction action = inputs[i].FindAction(rebinding.actionPath);
            if (action == null)
                continue;
            //if (rebinding.bindingIndex != int)
            //    action.ApplyBindingOverride((int)rebinding.bindingIndex + 1, rebinding.binding);
            //else
            action.ApplyBindingOverride(rebinding.binding, rebinding.group.name);
        }
    }
}

public readonly struct InputRebinding
{
    public readonly string actionPath;
    public readonly string binding;
    public readonly InputDevice group;
    public readonly int bindingIndex;

    [JsonConstructor]
    public InputRebinding(string actionPath, string binding, InputDevice group, int bindingIndex)
    {
        this.actionPath = actionPath;
        this.binding = binding;
        this.group = group;
        this.bindingIndex = bindingIndex;
    }

}