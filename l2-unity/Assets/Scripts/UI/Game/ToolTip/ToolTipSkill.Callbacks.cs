

public partial class ToolTipSkill
{
    public void RegisterCallbackActive(Dictionary<int, VisualElement> dict, SkillListWindow skillWindow)
    {

        //foreach (var item in dict)
        //{
            //VisualElement element = item.Value;
           // if (element != null)
           // {
              //  element.RegisterCallback<MouseOverEvent>(evt =>
               // {
                 //   VisualElement ve = (VisualElement)evt.currentTarget;
                  //  if (ve != null)
                   // {
                    //    if (DragAndDropManager.getInstance().IsDrag())
                     //   {
                     //       ResetElement(ve);
                     //   }
                      //  else
                      //  {
                         //////  int cellId = ParceCellId(ve.name);
                          //  Skillgrp skillGrp = skillWindow.GetSkillIdByCellId(1 , cellId);

                            //if (skillGrp != null)
                            //{
                              //  SkillNameData nameData = TablePacketServices.SkillGrps.GetSkillName(skillGrp.Id, skillGrp.Level);
                             //   if(nameData != null)
                               // {
                               //     SetData(skillGrp, nameData);
                               //     CalcNewPosition(ve);
                               // }
                                
                            //}
                            
                            
                        //}
                    //}
                //}, TrickleDown.TrickleDown);

                //element.RegisterCallback<MouseOutEvent>(evt =>
                //{
                //    VisualElement ve = (VisualElement)evt.currentTarget;
                 //   if (ve != null)
                  //  {
                  //      ResetElement(ve);
                  //  }
                  //  evt.StopPropagation();

               // }, TrickleDown.TrickleDown);

            //}
        //}
    }


    public void RegisterCallbackPassive(Dictionary<int, VisualElement> dict, SkillListWindow skillWindow)
    {

       // foreach (var item in dict)
        //{
            //VisualElement element = item.Value;
            //if (element != null)
            //{
                //element.RegisterCallback<MouseOverEvent>(evt =>
                //{
                   // VisualElement ve = (VisualElement)evt.currentTarget;
                   // if (ve != null)
                    //{
                        //if (DragAndDropManager.getInstance().IsDrag())
                        //{
                         //   ResetElement(ve);
                        //}
                        //else
                       // {
                            //int cellId = ParcePassiveCellId(ve.name);
                            //Skillgrp skillGrp = skillWindow.GetSkillIdByCellId(2, cellId);

                           // if (skillGrp != null)
                           // {
                                //SkillNameData nameData = TablePacketServices.SkillGrps.GetSkillName(skillGrp.Id, skillGrp.Level);
                                //if (nameData != null)
                                //{
                                 //   SetData(skillGrp, nameData);
                                 //   CalcNewPosition(ve);
                              //  }

                          //  }


                       // }
                  //  }
               // }, TrickleDown.TrickleDown);
//
               /// element.RegisterCallback<MouseOutEvent>(evt =>
               // {
                   // VisualElement ve = (VisualElement)evt.currentTarget;
                   // if (ve != null)
                   // {
                   //     ResetElement(ve);
                  //  }
                //    evt.StopPropagation();

             //   }, TrickleDown.TrickleDown);

           // }
        //}
    }

    private int ParceCellId(string name)
    {
        string strId = name.Replace("imgbox", "");
        return Int32.Parse(strId);
    }

    private int ParcePassiveCellId(string name)
    {
        string strId = name.Replace("pasbox", "");
        return Int32.Parse(strId);
    }

}
