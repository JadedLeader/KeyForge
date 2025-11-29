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
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { Button } from "@/components/ui/button"
import { toast } from "sonner"
import { Trash } from "lucide-react" 
import { DeleteTeamVault } from "@/components/api/TeamVault"

interface DeleteTeamVaultButtonProps { 
    teamVaultId: string;
}

function DeleteTeamVaultButton({teamVaultId } : DeleteTeamVaultButtonProps) {

    return (

        <div>

            <Button onClick={async () => {

                const deletedVault = await DeleteTeamVault(teamVaultId); 

                if (deletedVault.success) {
                    toast.success("Team vault has been deleted successfully!");
                }
                else {
                    toast.error("Something went wrong when trying to delete a team vault!");
                }

            }}></Button>

        </div>

  );
}

export default DeleteTeamVaultButton;