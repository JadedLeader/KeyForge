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
import {
    Table,
    TableBody,
    TableCaption,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import { toast } from "sonner"
import { GetPendingInvitesForAccount, UpdateTeamInvite } from "@/components/api/TeamInvite"
import {CreateTeamMember } from "@/components/api/TeamMember"


type TeamInvites = {
    inviteSentBy: string;
    inviteRecipient: string;
    inviteCreatedAt: string;
    teamInviteId: string;
    teamVaultId: string;
}
interface AccountPendingInvitesSegmentProps { 
    email: string;
}

function AccountPendingInvitesSegment({email } : AccountPendingInvitesSegmentProps) {

    const [pendingInvites, setPendingInvites] = useState<TeamInvites[]>([]);


    useEffect(() => {

        const gatheringPendingInvites = async () => {

            const pendingInvites = await GetPendingInvitesForAccount(email);

            if (pendingInvites.success && pendingInvites.pendingTeamInvites) {
                setPendingInvites(pendingInvites.pendingTeamInvites);
            } else {
                setPendingInvites([]);
            }

        }

        gatheringPendingInvites();


    }, [email])

    return (

        <Table>
            
            <TableHeader className="title-text">
                <TableRow>
                    <TableHead>Team Invite ID</TableHead>
                    <TableHead>Invite Sent By</TableHead>
                    <TableHead>Invite Recipient</TableHead>
                    <TableHead>Invite Created At</TableHead>
                    <TableHead>Invite Actions</TableHead>
                </TableRow>
            </TableHeader>

            <TableBody className="normal-text">
                {pendingInvites.length === 0 ? (
                    <TableRow>
                        <TableCell
                            colSpan={5}
                            className="text-center py-10 text-muted-foreground"
                        >
                            No pending invites.
                        </TableCell>
                    </TableRow>
                ) : (
                    pendingInvites.map((invites) => (
                        <TableRow key={invites.teamInviteId}>
                            <TableCell>{invites.teamInviteId}</TableCell>
                            <TableCell>{invites.inviteSentBy}</TableCell>
                            <TableCell>{invites.inviteRecipient}</TableCell>
                            <TableCell>{invites.inviteCreatedAt}</TableCell>
                            <TableCell>
                                <Button onClick={ async () =>
                                {

                                    const teamInviteUpdate = await UpdateTeamInvite(invites.teamInviteId, "Accepted"); 

                                    console.log("team vault id:", invites.teamVaultId);

                                    const createTeamMember = await CreateTeamMember(invites.teamVaultId , invites.teamInviteId);


                                }} variant="outline">Accept</Button>
                                <Button onClick={ () => UpdateTeamInvite(invites.teamInviteId, "Declined")} variant="outline" className="ml-2">
                                    Decline
                                </Button>
                            </TableCell>
                        </TableRow>
                    ))
                )}
            </TableBody>
        </Table>


        


  );
}

export default AccountPendingInvitesSegment;