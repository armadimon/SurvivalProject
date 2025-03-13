using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();  // UI????筌?六???嶺뚮㉡?€쾮?
    public void OnInteract();   // ?嶺뚮ㅏ援?????읐??嶺뚮ㅎ???
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    public string GetInteractPrompt()
    {
        string str = $"{itemData.disPlayName}\n{itemData.description}";
        return str;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.item = itemData;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }

}
