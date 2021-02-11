using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Represents a collection of methods that extent instances of types.
/// </summary>
public static class EditorExtensionMethods
{
    /// <summary>
    /// Gets all <see cref="SerializedProperty"/> contained in parent property.
    /// </summary>
    /// <param name="property">property containing children.</param>
    /// <returns><see cref="IEnumerable{SerializedProperty}"/> children.</returns>
    public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
    {
        property = property.Copy();
        var nextElement = property.Copy();
        bool hasNextElement = nextElement.NextVisible(false);
        if (!hasNextElement)
        {
            nextElement = null;
        }

        property.NextVisible(true);
        while (true)
        {
            if ((SerializedProperty.EqualContents(property, nextElement)))
            {
                yield break;
            }

            yield return property;

            bool hasNext = property.NextVisible(false);
            if (!hasNext)
            {
                break;
            }
        }
    }


    /// <summary>
    /// Uses rectangle to create a tile layout by amount.
    /// </summary>
    /// <param name="source">Tile sample.</param>
    /// <param name="amount">Amount of tiles to create.</param>
    /// <returns>Anchor points for Tiles.</returns>
    public static Vector2[] GetAsTileAnchors(this Rect source, int amount)
    {
        int row = 0;
        int column = 0;
        int breakpoint = amount / 2;
        List<Vector2> anchors = new List<Vector2>();

        for (int i = 0; i < amount; i++)
        {
            anchors.Add(new Vector2(column * source.width, row * source.height));
            column++;
            if (i == breakpoint)
            {
                row++;
                column = 0;
            }
        }
        return anchors.ToArray();
    }
}