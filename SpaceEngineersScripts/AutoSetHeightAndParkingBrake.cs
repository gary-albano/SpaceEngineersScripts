using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

public class Program : MyGridProgram
{
    bool IsInCockpit;
    bool NeedsUpdate;

    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }

    public void Main(string argument)
    {
        List<IMyTerminalBlock> cockpitList = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpitList);
        if (cockpitList.Count == 0)
        {
            Echo("Script Error:\n No Cockpits found");
            return;
        }

        IMyCockpit myCockpit = null;
        for (int i = 0; i < cockpitList.Count; i++)
        {
            if (((IMyCockpit)cockpitList[i]).IsMainCockpit)
                myCockpit = (IMyCockpit)cockpitList[i];
        }

        //couldnt find main cockpit so lets use first one
        if (myCockpit == null)
            myCockpit = (IMyCockpit)cockpitList[0];

        if (IsInCockpit != myCockpit.IsUnderControl)
        {
            IsInCockpit = myCockpit.IsUnderControl;
            NeedsUpdate = true;
        }

        if (!NeedsUpdate)
            return;

        //Set or unset parking brake
        myCockpit.HandBrake = myCockpit.IsUnderControl;

        List<IMyTerminalBlock> wheelList = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(wheelList);
        if (wheelList.Count == 0)
        {
            Echo("Script Error:\n No Wheel Suspensions found");
            return;
        }

        for (int i = 0; i < wheelList.Count; i++)
        {
            if (wheelList[i].CustomName == "Wheel Suspension 3x3")
            {
                if (myCockpit.IsUnderControl)
                    ((IMyMotorSuspension)wheelList[i]).Height = 0f;
                else
                    ((IMyMotorSuspension)wheelList[i]).Height = 1.3f;
            }
        }

        NeedsUpdate = false;
    }
}
