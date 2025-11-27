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
import { useState} from "react"
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
import { CreateTeamVault } from "@/components/api/TeamVault"

interface CreateTeamProps { 
    isOpen: boolean; 
    setIsOpen: React.Dispatch<React.SetStateAction<boolean>>;

}
export function CreateTeamModal({isOpen, setIsOpen } : CreateTeamProps) {

    const [teamName, setTeamName] = useState("");
    const [invites, setInvites] = useState("");
    const [memberCap, setMemberCap] = useState<number>(0);
    const [inviteDropOpen, setInviteDropOpen] = useState<Boolean>();
    const [teamVaultDescription, setTeamVaultDescription] = useState("");
    const [teamVaultName, setTeamVaultName] = useState(""); 
    const [currentState, setCurrentState] = useState("");
    const [stateDropOpen, setStateDropOpen] = useState<boolean>();

    function HandleTeamNameChange(teamName: React.ChangeEvent<HTMLInputElement>) { 

        setTeamName(teamName.target.value);
    }

    function HandleMemberCapChange(memberCap: React.ChangeEvent<HTMLInputElement>) {
        setMemberCap(Number(memberCap.target.value));
    }

    function HandleTeamVaultNameChange(teamVaultName: React.ChangeEvent<HTMLInputElement>) { 
        setTeamVaultName(teamVaultName.target.value);

    }

    function HandleTeamVaultDescriptionChange(teamVaultDescription: React.ChangeEvent<HTMLInputElement>) { 
        setTeamVaultDescription(teamVaultDescription.target.value);
    }


  return (
   

      <Dialog open={isOpen} onOpenChange={setIsOpen} >

          <DialogContent className="expand-label-theme expand-label-theme:hover">

              <DialogTitle className="title-text">

                    Create Team

              </DialogTitle>

              <DialogDescription className="normal-text">

                    This view allows for you to create a team so you can collaborate with others!

              </DialogDescription>

              <Label className="title-text">Team Name:</Label>
              <Input value={teamName} placeholder="Team Name" onChange={HandleTeamNameChange} type="text" ></Input>

              <Label className="title-text">Invites:</Label>
              <div className={inviteDropOpen ? "mb-15" : "mb-4"} >
                  <Select value={invites} onOpenChange={(open) => setInviteDropOpen(open)} onValueChange={(value) =>
                  {
                      setInvites(value); 
                      setInviteDropOpen(false);
                  }} >

                  <SelectTrigger>

                  <SelectValue placeholder="Invitations"></SelectValue>
                  </SelectTrigger>

                      <SelectContent >

                            <SelectItem className="normal-text bg-transparent hover::underline border-0" value="closed">Closed</SelectItem>
                            <SelectItem className="normal-text bg-transparent hover::underline border-0" value="open">Open</SelectItem>

                      </SelectContent>

                  </Select>
              </div>

              <Label className="title-text">Maximum Allowed Members:</Label>
              <Input value={memberCap} placeholder="Member Capacity" onChange={HandleMemberCapChange} type="number" ></Input>

              <Label className="title-text">Team Vault Name:</Label>
              <Input value={teamVaultName} onChange={HandleTeamVaultNameChange} type="text" ></Input>

              <Label className="title-text">Team Vault Description:</Label>
              <Input value={teamVaultDescription} onChange={HandleTeamVaultDescriptionChange} type="text" ></Input>

              <div className={stateDropOpen ? "mb-15" : "mb-4"} >
                  <Select value={currentState} onOpenChange={(open) => setStateDropOpen(open)} onValueChange={(state) =>
                  {
                      setCurrentState(state);
                      setStateDropOpen(false);
                  }} >

                      <SelectTrigger>
                          <SelectValue placeholder="Vault Status" />
                      </SelectTrigger>

                      <SelectContent  >

                          <SelectItem className="normal-text bg-transparent hover::underline border-0" value="read only">Read only</SelectItem>
                          <SelectItem className="normal-text bg-transparent hover::underline border-0" value="Editable">Editable</SelectItem>

                      </SelectContent>


                    </Select>
              </div>

              <DialogFooter >

                  <DialogClose asChild>

                      <Button className="bg-blue-600 text-white hover::underline" variant="outline">Close</Button>

                  </DialogClose>

                  <Button className="bg-blue-600 text-white hover::underline" variant="outline" onClick={async () => {


                      const teamCreated = await CreateTeam(teamName, invites, memberCap);

                      const teamVaultCreated = await CreateTeamVault(teamCreated.teamId, teamVaultDescription, teamVaultName, currentState);
                      

                      if (teamCreated.success && teamVaultCreated.success) {
                          toast.success("Team created successfully!");

                          setIsOpen(false);
                      }
                      else { 
                          toast.error("An error ocurred, please try again!");
                      }

                  }}>Create Team</Button>

              </DialogFooter>


          </DialogContent>


      </Dialog>



    );




}

export default CreateTeamModal;