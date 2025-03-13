using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();  // UI????戮?뻣???筌먲퐢沅?
    public void OnInteract();   // ?筌뤿굛????댟??筌뤾쑵??
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
