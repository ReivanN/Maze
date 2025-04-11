using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttributeManager : MonoBehaviour
{
    public const int MaxActiveAttributes = 3;

    public static List<Atribute> activeAttributes = new List<Atribute>();

    public static List<Atribute> GetAvailableAttributes()
    {
        return AtributeDataBase.allAttributes.OrderBy(u => Random.value).Take(MaxActiveAttributes).ToList();
    }

    public static bool CanAddAttribute(Atribute attribute)
    {
        return !activeAttributes.Contains(attribute);
    }

    public static bool IsFull()
    {
        return activeAttributes.Count >= MaxActiveAttributes;
    }

    public static void ReplaceAttribute(Atribute attributeToRemove, Atribute newAttribute)
    {
        int index = activeAttributes.IndexOf(attributeToRemove);
        if (index != -1 && !activeAttributes.Contains(newAttribute))
        {
            activeAttributes[index] = newAttribute;
        }
    }

    public static void AddAttribute(Atribute newAttribute)
    {
        if (!activeAttributes.Contains(newAttribute))
        {
            if (activeAttributes.Count < MaxActiveAttributes)
            {
                activeAttributes.Add(newAttribute);
            }
            else
            {
                Debug.LogWarning("Слотов атрибутов больше нет. Используйте ReplaceAttribute.");
            }
        }
    }

    public static void RemoveAttribute(Atribute attribute)
    {
        activeAttributes.Remove(attribute);
    }
}
