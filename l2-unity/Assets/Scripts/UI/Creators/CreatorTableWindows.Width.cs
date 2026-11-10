using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTableWindows
{
    public void SetManualWidth(VisualElement containerBgElement, float width)
    {
        if (width > 0)
        {
            containerBgElement.style.flexBasis = width;    
            containerBgElement.style.flexGrow = 1;       
            containerBgElement.style.flexShrink = 0;
        }
    }

    private void ChangeWidthListView(IEnumerable<VisualElement> elementsListView ,  float[] allWith)
    {
        foreach (var item in elementsListView)
        {
  
            var row = item;
            var columns = row.Children().ToList(); 

            for(int i = 0; i < allWith.Length; i++)
            {
                var column = columns[i];
                var new_width = allWith[i];

                column.style.flexBasis = new_width;
                column.style.flexShrink = 0;
                column.MarkDirtyRepaint();
            }
        }
    }

}
