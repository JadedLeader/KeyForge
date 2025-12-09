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
import { GetTeamVault } from "@/components/api/TeamVault"
import { UseTeamMembers } from "@/components/Hubs/TeamMemberHub"

interface TeamVaultDashboardProps { 

    teamId: string;

}

interface TeamMemberAdded {
    username: string;
    email: string;
}

function TeamVaultDashboard({teamId } : TeamVaultDashboardProps) {

    const [recipientEmail, setRecipientEmail] = useState("");

    const [teamVaultId, setTeamVaultId] = useState("");

    useEffect(() => {

        const loadTeamVault = async () => {

            const teamVault = await GetTeamVault(teamId);

            if (teamVault.success) {
                setTeamVaultId(teamVault.teamVaultId);
            }

        }

        loadTeamVault();

    }, [])

    const teamMembersAdded = UseTeamMembers(teamVaultId);

    function RecipientEmailOnChange(e: React.ChangeEvent<HTMLInputElement>) { 
        setRecipientEmail(e.target.value);
    }

    return (

        <div className="p-4">

            <div className="grid grid-cols-1 xl:grid-cols-[2fr_1fr] gap-6">

                <div className="bg-neutral-900 border border-neutral-700 rounded-xl p-6">

                    <Label className="normal-text">This is where keys will be held</Label>

                </div>

                <div className="bg-neutral-900 border border-neutral-700 rounded-xl p-6">

                    <h1 className="title-text">Members</h1>

                    <h3 className="text-lg font-semibold mb-2 title-text">Invite Someone</h3>

                    <div className="flex gap-2">
                        <Input
                            value={recipientEmail}
                            onChange={RecipientEmailOnChange}
                            type="email"
                            placeholder="Enter email"
                            className="flex-1 bg-neutral-800 p-2 rounded border border-neutral-600 text-white"
                        />

                        <Button variant="outline" className="bg-blue-600 px-4 py-2 rounded hover:bg-blue-500"
                            onClick={async () =>
                            {

                                const teamInvite = await CreateTeamInvite(teamVaultId, recipientEmail); 

                                if (teamInvite.success) {
                                    toast.success(`Team invite sent to ${recipientEmail} succesfully!`);
                                }
                                else { 
                                    toast.error(`Invite could not be sent, please try again`);
                                }

                            }} >
                            Invite
                        </Button>
                    </div>

                </div>

            </div>

        </div>

  );
}

export default TeamVaultDashboard;