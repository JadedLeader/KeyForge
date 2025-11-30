import React from "react";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogTitle,
    DialogFooter,
    DialogClose
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { useEffect, useState } from "react"
import { Input } from "@/components/ui/input"
import { CreateTeam } from "@/components/api/Team"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { toast } from "sonner"
import { CreateTeamInvite } from "@/components/api/TeamInvite"

interface CreateTeamInviteProps { 
    teamVaultId: string;
}

function CreateTeamInviteSegment({teamVaultId} : CreateTeamInviteProps) {

    const [emailOfRecipient, setEmailOfRecipient] = useState("");
    

    function HandleEmailChange(e: React.ChangeEvent<HTMLInputElement>) { 

        setEmailOfRecipient(e.target.value);

    }

  return (

      <div>

          <Label className="title-text">Send Invite:</Label>

          <Input value={emailOfRecipient} onChange={HandleEmailChange} placeholder="Email" type="email"></Input>

          <Button onClick={ async () =>
          {
              const teamInvite = await CreateTeamInvite(teamVaultId, emailOfRecipient);

              if (teamInvite.success) {
                  toast.success(`Invite sent to: ${emailOfRecipient}`);
              }
              else { 
                  toast.error(`Invite to ${emailOfRecipient} has failed, please try again`);
              }

          }}
          >Send Invite</Button>

      </div >

    );




}

export default CreateTeamInviteSegment;