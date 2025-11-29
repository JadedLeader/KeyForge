import React from "react"
import { useState, useEffect } from "react"
import { Input } from "@/components/ui/input"
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
    DialogFooter,
    DialogClose
} from "@/components/ui/dialog"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { Button } from "@/components/ui/button"
import { toast } from "sonner"
import { Trash } from "lucide-react"
import { UpdateTeamVault, GetTeamVault } from "@/components/api/TeamVault"

interface UpdateTeamVaultViewProps { 
    teamVaultId: string;
}

const Status = {
    Editable: "editable",
    Readonly: "readonly",
} as const;

type Status = (typeof Status)[keyof typeof Status];
function UpdateTeamVaultView({teamVaultId } : UpdateTeamVaultViewProps) {

    const [teamVaultName, setTeamVaultName] = useState("");
    const [teamVaultDescription, setTeamVaultDescription] = useState("");
    const [newStatus, setNewStatus] = useState<Status | "">("");

    useEffect(() => { 

        const loadTeamVault = async () => {

            const teamVault = await GetTeamVault(teamVaultId);

            if (teamVault.success) {

                setTeamVaultName(teamVault.teamVaultName);
                setTeamVaultDescription(teamVault.teamVaultDescription);
            }
        };

        loadTeamVault();

    }, [])

    function HandleTeamVaultNameChange(e: React.ChangeEvent<HTMLInputElement>) { 
        setTeamVaultName(e.target.value);
    }

    function HandleTeamVaultDescriptionChange(e: React.ChangeEvent<HTMLInputElement>) { 
        setTeamVaultDescription(e.target.value);
    }


  return (

      <Dialog>

          <DialogContent>

              <DialogHeader>

                  <DialogTitle className="title-text">Update Team Vault</DialogTitle>

                  <DialogDescription>This view is used to update the team vault!</DialogDescription>

                  <Label className="title-text">Team Vault Name:</Label>
                  <Input size={25} className="normal-text" value={teamVaultName} placeholder={teamVaultName} onChange={HandleTeamVaultNameChange}></Input>

                  <Label className="title-text">Team Vault Description:</Label>
                  <Input className="normal-text" value={teamVaultDescription} placeholder={teamVaultDescription} onChange={HandleTeamVaultDescriptionChange}></Input>

                  <Select value={newStatus} onValueChange={(status : Status) => setNewStatus(status)} >

                      <SelectTrigger>

                          <SelectValue className="normal-text">Status</SelectValue>

                          <SelectItem value={Status.Editable}>Editable</SelectItem>
                          <SelectItem value={Status.Readonly}>Readonly</SelectItem>

                      </SelectTrigger>

                  </Select>


              </DialogHeader>


          </DialogContent>

      </Dialog>

  );
}

export default UpdateTeamVaultView;