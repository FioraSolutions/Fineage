using System.Collections.Generic;

public partial class ShortCutInitDto : IWireDto
{
    public ShortCutInitDto()
    {
        _shortcut = new List<Shortcut>();
    }

    public void ReadFrom(PacketReader reader)
    {
        int size = reader.ReadI();
        Shortcut shortcut;

        for (int i = 0; i < size; i++)
        {
            int type = reader.ReadI();
            int world_slot = reader.ReadI();

            int slot = world_slot % 12;
            int page = world_slot / 12;

            if (type == Shortcut.TYPE_ITEM)
            {
                int itemCutId = reader.ReadI();
                reader.ReadI();
                reader.ReadI();
                reader.ReadI();
                reader.ReadI();
                reader.ReadSh();
                reader.ReadSh();
                shortcut = new Shortcut(slot, page, Shortcut.TYPE_ITEM, itemCutId, 0);
                _shortcut.Add(shortcut);

            }
            else if (type == Shortcut.TYPE_SKILL)
            {
                int itemCutId = reader.ReadI();
                int level = reader.ReadI();
                //shortcut = new Shortcut(shortCutId, level);
                reader.ReadB();// C5
                reader.ReadI();// C6
                shortcut = new Shortcut(slot, page, Shortcut.TYPE_SKILL, itemCutId, level);
                _shortcut.Add(shortcut);
            }
            else if (type == Shortcut.TYPE_ACTION)
            {
                int actionId = reader.ReadI();
                _shortcut.Add(new Shortcut(slot, page, Shortcut.TYPE_ACTION, actionId, 0));
                reader.ReadI();// C6
            }
            else if (type == Shortcut.TYPE_MACRO)
            {
                //int macroCutId = reader.ReadI();
                //shortcut = new Shortcut(slot, page, Shortcut., macroCutId, 0);
                reader.ReadI();// C6
            }
            else if (type == Shortcut.TYPE_RECIPE)
            {
                int shortCutId = reader.ReadI();
                reader.ReadI();// C6
            }

        }
    }
}
