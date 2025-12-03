import React from "react"
import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import "./HomePage.css"
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
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
import {
    Field,
    FieldContent,
    FieldDescription,
    FieldError,
    FieldGroup,
    FieldLabel,
    FieldLegend,
    FieldSeparator,
    FieldSet,
    FieldTitle,
} from "@/components/ui/field"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { ChevronRight, MoreVertical, Trash2, Edit, Eye, EyeOff, Cog, Pencil, Trash, Maximize2 } from "lucide-react" 
import {
    ResizableHandle,
    ResizablePanel,
    ResizablePanelGroup,

} from "@/components/ui/resizable"
import {
    Item,
    ItemActions,
    ItemContent,
    ItemDescription,
    ItemFooter,
    ItemHeader,
    ItemMedia,
    ItemTitle,
    ItemGroup
} from "@/components/ui/item"
import { Separator } from "../src/components/ui/separator"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { toast } from "sonner"
import { EditVaultModal } from "@/components/Vaults/EditVaultModal"
import { GetUserAccountDetails } from "@/components/api/Account"
import { SilentTokenRefresh } from "@/components/api/Auth"
import { DeleteVault, CreateNewVault } from "@/components/api/Vault"
import { DeleteAllKeysFromVault, DecryptVaultKey, CreateNewVaultKey, GetVaultsAndKeys, GetVaultWithAllDetails } from "@/components/api/VaultKeys"
import { CreateTeamModal } from "@/components/Team/CreateTeamModal"
import {GetTeams } from "@/components/api/Team"
import TeamVaultDashboard from "../src/components/Team/TeamVaultDashboard"
import {UseTeamInvites } from "@/components/Hubs/TeamInvitesHub"
import AccountPendingInvitesSegment from "@/components/TeamInvite/AccountPendingInvitesSegment"


interface CreateVaultWithKeysResponse { 

    vaultId: string; 
    vaultName: string;
    success: boolean

}

interface DeleteVaultWithAllKeysResponse { 

    vaultId: string; 
    accountId: string;
    sucessful: boolean;

}

type Vault = { 
    vaultId: string;
    vaultName: string;
    vaultCreatedAt: string;
    vaultType: string;
    keys: VaultKey[]
}

type VaultKey = { 
    vaultKeyId: string; 
    keyName: string; 
    hashedVaultKey: string; 
    dateTimeVaultKeyCreated: string;
}

type Team = {
    id: string;
    teamName: string;
}

interface SetPropsForDeletionVerificaitonModal { 
    isOpen: boolean; 
    setIsOpen: React.Dispatch<React.SetStateAction<boolean>>;

    vaultId: string;
    reloadVaults: () => void;
}

export function CreateDeleteVerificationModal({ isOpen, setIsOpen, vaultId, reloadVaults }: SetPropsForDeletionVerificaitonModal) { 



    function BuildDeleteVaultWithAllKeys(vaultId: string, accountId: string, sucessful: boolean): DeleteVaultWithAllKeysResponse { 

        const buildingResponse: DeleteVaultWithAllKeysResponse = {
            vaultId: vaultId,
            accountId: accountId, 
            sucessful: sucessful
            
        }; 

        return buildingResponse;

    }

    async function DeleteVaultWithAllKeys() : Promise<DeleteVaultWithAllKeysResponse> { 


        console.log("hit final", vaultId);

        const deleteAllKeysWithinVault = await DeleteAllKeysFromVault(vaultId); 

        const deleteVault = await DeleteVault(vaultId);


        if (!deleteVault.sucessful) { 

            throw new Error("Deleting vault was not successful");

        }

        console.log("vault has been deleted with all keys"); 

        const buildingResponse = BuildDeleteVaultWithAllKeys(deleteVault.vaultId, deleteVault.accountId, deleteVault.sucessful);

        return buildingResponse;


    }

    return (

        <div>

            <Dialog open={isOpen} onOpenChange={setIsOpen} >
                <DialogContent className="expand-label-theme expand-label-theme-hover overflow-y-auto">
                    <DialogHeader>
                        <DialogTitle>Are you sure you want to delete this vault and all of it's keys?</DialogTitle>
                        <DialogDescription>
                            This action cannot be undone. This will permanently delete your vault
                            and remove your all data.
                        </DialogDescription>
                    </DialogHeader>

                    <DialogFooter>

                        <DialogClose asChild>
                            <Button className="bg-blue-600 text-white hover::underline" variant="outline">Close</Button>
                        </DialogClose>

                        <Button className="bg-blue-600 text-white hover::underline" variant="outline"
                            onClick={async () =>
                            {
                                const deleting = await DeleteVaultWithAllKeys();

                                if (deleting.sucessful) {
                                    toast.success(`Vault: ${deleting.vaultId} has been deleted!`);
                                    setIsOpen(false);
                                    reloadVaults();
                                }
                                else { 
                                    toast.error(`Vault ${deleting.vaultId} could not be deleted`);
                                    setIsOpen(false);
                                }

                            }}>
                            Delete
                        </Button>

                    </DialogFooter>

     

                </DialogContent>
            </Dialog>
            
             
        </div>

    ); 

}

interface ExpandVaultDetailsProps { 
    vaultId: string;
    vaultName: string;
    vaultKeys: VaultKey[];
    isOpen: boolean; 
    setIsOpen: () => void;
}

export function ExpandVaultDetails({vaultName, vaultId, vaultKeys, isOpen, setIsOpen }: ExpandVaultDetailsProps) { 
 
    useEffect(() => {

        const loadVaultDetails = async () => {

            await GetVaultWithAllDetails(vaultId);

        };

        loadVaultDetails();

    }, []);


    return (

        <Dialog open={isOpen} onOpenChange={setIsOpen}>
            <DialogContent className="expand-label-theme expand-label-theme-hover max-w-lg max-h-[80vh] overflow-y-auto p-6 rounded-lg">
                <DialogHeader>
                    <DialogTitle>Vault: {vaultName}</DialogTitle>
                    <DialogDescription>
                        Details of all vault keys in this vault
                    </DialogDescription>
                </DialogHeader>

                <div className="mt-4 space-y-3">
                    {vaultKeys.map((key) => (
                        <div
                            key={key.vaultKeyId}
                            className="border-b border-gray-700 pb-2"
                        >
                            <Label className="title-text">Vault Key ID: {key.vaultKeyId}</Label>
                        
                            <Label className="title-text ">Key Name: {key.keyName}</Label>

                            <Label className="title-text">Encrypted Key: {key.hashedVaultKey}</Label>

                            <Label className="title-text">Vault Key Created: {key.dateTimeVaultKeyCreated}</Label>
                        </div>
                    ))}
                </div>

                <DialogFooter className="mt-4">
                    <DialogClose asChild>
                        <Button variant="outline">Close</Button>
                    </DialogClose>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );


}

interface VaultDashboardProps { 
    vaults: Vault[];
    reloadVaults: () => void;
}

export function VaultDashboard({ vaults, reloadVaults }: VaultDashboardProps) {

    const [decryptedKey, setDecryptedKey] = useState<Record<string, string>>({});
    const [deleteOpen, setDeleteOpen] = useState<string | null>(null);
    const [isExpandOpen, setIsExpandOpen] = useState<string | null>();
    const [editOpen, setEditOpen] = useState<string | null>();


    const handleRevealKey = async (encryptedKey: string, vaultId: string, vaultKeyId: string) => {


        const response = await DecryptVaultKey(encryptedKey, vaultId);

            setDecryptedKey(prev => ({
                ...prev,
                [vaultKeyId]: response.decryptedVaultKey,
            }));
        
    };

   
    
    return (
        <div className="flex flex-wrap gap-4 rounded-2xl 
        shadow-lg shadow-black/40 hover:shadow-xl hover:shadow-black/60 
         p-4">

            {vaults.map((vault) => (

                <Card key={vault.vaultId} className=" group card-theme w-80 card-theme-hover">
                    <CardHeader>
                        <CardTitle className="flex justify-between font-semibold text-neutral-50">

                            <Cog className="transition-transform duration-300 group-hover:rotate-180" />
                            Vault: {vault.vaultName}

                            <DropdownMenu >
                                <DropdownMenuTrigger>

                                    <MoreVertical />


                                </DropdownMenuTrigger>

                                <DropdownMenuContent className="text-gray-400 bg-transparent border border-neutral-600 bg-neutral-900 p-1 min-w-0">

                                    <DropdownMenuItem className="p-1 h-6" onClick={() => setEditOpen(vault.vaultId)}>
                                        <Pencil size={16}  /> 
                                    </DropdownMenuItem >
                                    <DropdownMenuItem className="p-1 h-6" onClick={() => setDeleteOpen(vault.vaultId)}  >
                                        <Trash size={16}  />
                                    </DropdownMenuItem>

                                    
                                    <DropdownMenuItem className="p-1 h-6" onClick={() => setIsExpandOpen(vault.vaultId) } >
                                        <Maximize2 size={16}  />
                                    </DropdownMenuItem>

                                </DropdownMenuContent>

                                

                            </DropdownMenu>
                        </CardTitle>
               
                    </CardHeader>

                    <CreateDeleteVerificationModal isOpen={deleteOpen === vault.vaultId} setIsOpen={() => setDeleteOpen(null)} vaultId={vault.vaultId} reloadVaults={reloadVaults} />
                   
                    {editOpen === vault.vaultId  && (
                        <EditVaultModal vaultId={vault.vaultId} reloadVaults={reloadVaults} isOpen={!!editOpen} setIsOpen={() => setEditOpen(null)} currentVaultName={vault.vaultName} />
                    )}

                    {isExpandOpen === vault.vaultId && (
                        <ExpandVaultDetails
                            vaultName={vault.vaultName}
                            vaultId={vault.vaultId}
                            vaultKeys={vault.keys}
                            isOpen={isExpandOpen === vault.vaultId}
                            setIsOpen={() => setIsExpandOpen(null)}
                        />
                    )}
                    

                    <CardContent className="space-y-2">
                        {vault.keys.map((key) => (
                            <div key={key.vaultKeyId}>

                                <div className="flex justify-between">
                                    <FieldLabel className="title-text">Key Name:</FieldLabel>
                                <p className="normal-text">{key.keyName}</p>
                                </div>

                                
                                <FieldLabel className="title-text">Vault Key(s)</FieldLabel>

                                <div className="flex justify-between"> 
                                    <p className="normal-text">{decryptedKey[key.vaultKeyId] ?? key.hashedVaultKey} </p>
                                    <Button onClick={() => handleRevealKey(key.hashedVaultKey, vault.vaultId, key.vaultKeyId)} >
                                        <Eye/>
                                    </Button>
                                </div>
                                
                            </div>
                        ))}
                    </CardContent>

                </Card>

            ))}
        </div>
    );
}

interface BuildCreateVaultModalProps { 
    dialogOpen: boolean; 
    setDialogOpen: React.Dispatch<React.SetStateAction<boolean>>;

    reloadVaults: () => void;

    teams: Team[];

}

export function BuildCeateVaultModal({dialogOpen, setDialogOpen, reloadVaults, teams} : BuildCreateVaultModalProps)  { 


    const [vaultName, setVaultName] = useState("");
    const [vaultType, setVaultType] = useState("");
    const [keyName, setKeyName] = useState("");
    const [keyPassword, setKeyPassword] = useState("");
    const [dropDownOpen, setDropdownOpen] = useState(false);
    function HandleKeyNameChanged(e: React.ChangeEvent<HTMLInputElement>) { 
        setKeyName(e.target.value);
    }

    function HandleKeyPasswordChanged(e: React.ChangeEvent<HTMLInputElement>) { 
        setKeyPassword(e.target.value);
    }

    function HandleVaultNameChanged(e: React.ChangeEvent<HTMLInputElement>) {

        setVaultName(e.target.value);
    }

    function HandleDropdownOpen(open: boolean) { 
        setDropdownOpen(open);
    }

    function HandleVaultTypeChanged(vaultType: string) {
        setVaultType(vaultType);
    }


    function BuildVaultWithKeysResponse(vaultId: string, vaultName: string, success: boolean): CreateVaultWithKeysResponse { 

        const buildingVaultWithKeysResponse: CreateVaultWithKeysResponse = {

            vaultId: vaultId,
            vaultName: vaultName,
            success : success


        }; 

        return buildingVaultWithKeysResponse;

    }

    

    async function CreateVaultWithKey(vaultName: string, keyName: string, keyPassword: string): Promise<CreateVaultWithKeysResponse> { 

        const buildVault = await CreateNewVault(vaultName);

        const buildKeys = await CreateNewVaultKey(keyName, keyPassword, buildVault.vaultId);

        const buildingResponse = BuildVaultWithKeysResponse(buildVault.vaultId, buildKeys.vaultName, buildKeys.success); 

        return buildingResponse;

    }

    return (

        
        <Dialog open={dialogOpen} onOpenChange={ setDialogOpen}  >
            <DialogContent className="expand-label-theme expand-label-theme:hover overflow-y-auto ">

                    <DialogHeader >

                        <DialogTitle >Create Vault</DialogTitle>

                    </DialogHeader>

                    <Label>Vault Name</Label>
                <Input value={vaultName} onChange={HandleVaultNameChanged} maxLength={20} />

                <Label>Key Name</Label>
                <Input value={keyName} onChange={HandleKeyNameChanged} maxLength={20} />

                    <Label>Password</Label>
                <Input value={keyPassword} onChange={HandleKeyPasswordChanged} maxLength={20} />


                    <DialogFooter >

                        <DialogClose asChild>
                            <Button className="bg-blue-600 text-white hover::underline" variant="outline" > Close </Button>
                        </DialogClose>

                    <Button
                        className="bg-blue-600 text-white hover:underline"
                        variant="outline"
                        onClick={async () => {

                           
                                const result = await CreateVaultWithKey(vaultName, keyName, keyPassword);

                                if (result.success) {

                                    console.log("Vault created");

                                    toast.success("Vault has been created!");

                                    HandleDropdownOpen(false);

                                    setDialogOpen(false);

                                    reloadVaults();

                                    setVaultName("");
                                    setVaultType("");
                                    setKeyName("");
                                    setKeyPassword("");


                                }
                           
                            
                            
                        }}
                    >
                        Create Vault
                    </Button>

                    </DialogFooter>

            </DialogContent>
        </Dialog>

    );
    


}


interface BuildAvatarAndUsernameProps { 
    username: string; 
    email: string;
}
export function BuildAvatarAndUsernameSideBarSegment({username, email } : BuildAvatarAndUsernameProps) { 

    return (

        <div className="flex flex-row pt-2 items-start">
            <Avatar className="bg-gray-700 font-semibold text-neutral-50">
                <AvatarFallback>{username.charAt(0).toUpperCase()}</AvatarFallback>
            </Avatar>

            <div className="flex flex-col ml-3">
                <Label className="font-semibold text-neutral-50">{username}</Label>
                <Label className="font-semibold text-neutral-50 mt-1">{email}</Label>
            </div>
        </div>

    );

}

export function HomePage() {

    const [vaults, setVaults] = useState<Vault[]>([]);
    const [shortLivedToken, setShortLivedToken] = useState("");
    const [isCreateVaultDialogOpen, setIsCreateVaultDialogOpen] = useState(false);
    const [vaultDropdownOpen, setVaultDropDownOpen] = useState(false);
    const [createTeamOpen, setIsCreateTeamOpen] = useState(false);
    const [teamsWithNoVaults, setTeamsWithNoVaults] = useState<Team[]>([]);
    const [teams, setTeams] = useState<Team[]>([]);
    const [teamDropDownOpen, setTeamDropDownOpen] = useState(false);
    const [selectTeamVaultId, setSelectedTeamVaultId] = useState<string | null>(null);
    const [selectedTeamName, setSelectedTeamName] = useState<string | null>(null);
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [inviteOverview, setInviteOverview] = useState(false);

    const handleTeamClick = (team :Team) => { 
        setSelectedTeamVaultId(team.id);
        setSelectedTeamName(team.teamName);
        setInviteOverview(false);
    }
   
    useEffect(() => {

        const fetchToken = async () => {

            const shortLivedToken = await SilentTokenRefresh();

            setShortLivedToken(shortLivedToken.refreshedToken);

            const vaults = await GetVaultsAndKeys();

            setVaults(vaults);

            const teamResponse = await GetTeams();

            setTeams(teamResponse);

            const usernameAndEmail = await GetUserAccountDetails();

            setUsername(usernameAndEmail.username); 
            setEmail(usernameAndEmail.email);

        }; 

        fetchToken();




    }, []);

    const teamInvites = UseTeamInvites(email);

    useEffect(() => { 

        if (teamInvites.length === 0) { 
            return;
        }

        const latestNotification = teamInvites[teamInvites.length - 1];

        toast.success(`${latestNotification.inviteSentBy} has invited you to join a team!`);

    }, [teamInvites])

    return (
        <div className="flex relative h-screen">
            <ResizablePanelGroup direction="horizontal" className="h-full overflow-hidden">

                <ResizablePanel defaultSize={10} className="sidepanel-theme sidepanel-theme-hover overflow-hidden">
                    <div className="flex flex-col h-full">

                        <div className="flex flex-col">

                            <BuildAvatarAndUsernameSideBarSegment username={username} email={email} />

                            <Separator className="my-4 bg-[hsl(210,12%,12%)]" />

                            <div className={vaultDropdownOpen ? "mb-20" : "mb-6" }>
                                <Label className="title-text px-2 mb-1 block">Dashboard</Label>

                                <DropdownMenu onOpenChange={setVaultDropDownOpen}>
                                    <DropdownMenuTrigger className="justify-start text-blue-500 text-left normal-text px-4 hover:underline">
                                        My Personal Vaults
                                    </DropdownMenuTrigger>

                                    <DropdownMenuContent className="w-56 px-4" align="start"  >
                                        <DropdownMenuSeparator className="border-neutral-700" />
                                        <DropdownMenuItem
                                            onClick={() => setIsCreateVaultDialogOpen(true)}
                                            className="bg-transparent normal-text hover:underline"
                                        >
                                            Create Vault
                                        </DropdownMenuItem>
                                        <DropdownMenuSeparator />

                                        <DropdownMenuItem
                                            className="bg-transparent normal-text hover:underline"
                                            onClick={() =>
                                            {
                                                setSelectedTeamVaultId(null);
                                                setInviteOverview(false);

                                            } }
                                        >
                                            Vault Overview
                                        </DropdownMenuItem>
                                    </DropdownMenuContent>
                                </DropdownMenu>
                            </div>

                            <Separator className="my-4 bg-[hsl(210,12%,12%)]" />


                            <div className={teamDropDownOpen ? "mb-45" : "mb-6" }>
                                <Label className="title-text px-2 mb-1 block">Teams</Label>

                                <DropdownMenu onOpenChange={(open) => setTeamDropDownOpen(open)} >
                                    <DropdownMenuTrigger className="justify-start text-blue-500 text-left normal-text px-4 hover:underline">
                                        Manage Teams
                                    </DropdownMenuTrigger>

                                    <DropdownMenuContent className="w-56 px-4" align="start">
                                        <DropdownMenuItem
                                            className="bg-transparent normal-text hover:underline"
                                            onClick={() => setIsCreateTeamOpen(true)}
                                        >
                                            Create Team
                                        </DropdownMenuItem>
                                        <DropdownMenuSeparator />

                                        <DropdownMenuItem className="bg-transparent normal-text hover:underline">
                                            Add Team Member
                                        </DropdownMenuItem>
                                        <DropdownMenuSeparator />

                                        <DropdownMenuItem className="bg-transparent normal-text hover:underline">
                                            Team Invitations
                                        </DropdownMenuItem>
                                        <DropdownMenuSeparator />

                                        <DropdownMenuItem className="bg-transparent normal-text hover:underline">
                                            Remove Team Member
                                        </DropdownMenuItem>

                                    </DropdownMenuContent>
                                </DropdownMenu>

                             
                            </div>

                            <Separator className="my-4 bg-[hsl(210,12%,12%)]" />

                            <div>
                                <Label className="title-text mb-6 px-2">Team Names:</Label>

                                <ItemGroup className="flex flex-col p-1 ">
                                    {teams.map((team) => (
                                        

                                        <Item
                                            key={team.id}
                                            asChild
                                            className="normal-text bg-transparent hover:underline"
                                            onClick={() => handleTeamClick(team)}
                                        >

                                            <Button onClick={() => console.log("Team id clicked", team.id)} className="justify-start text-left normal-text">{team.teamName}</Button>

                                        </Item>
                                    ))}
                                </ItemGroup>

                            </div>

                            <Separator className="my-4 bg-[hsl(210,12%,12%)]" />

                            <div>

                                <Label className="title-text mb-6 px-2">Invites</Label>

                                <Button className="justify-start text-left normal-text" onClick={() => setInviteOverview(true)} >Pending Invites</Button>

                            </div>


                        </div>

                        <div className="mt-auto px-2 pb-4">
                            <Separator className="my-4 bg-[hsl(210,12%,12%)]" />

                            <Label className="title-text px-2 mb-2 block">Settings</Label>

                            <Button
                                className="w-full justify-start text-left px-4 py-2 text-blue-500 text-white hover:underline"
                                onClick={() => console.log("Open Settings")}
                            >
                                Account Settings
                            </Button>

                            <Button
                                className="w-full justify-start text-left px-4 py-2 text-blue-500 text-white hover:underline"
                                onClick={() => console.log("Open Preferences")}
                            >
                                Preferences
                            </Button>
                        </div>
                    </div>

             
                    <BuildCeateVaultModal
                        teams={teamsWithNoVaults}
                        dialogOpen={isCreateVaultDialogOpen}
                        setDialogOpen={setIsCreateVaultDialogOpen}
                        reloadVaults={async () => {
                            const getVaults = await GetVaultsAndKeys();
                            setVaults(getVaults);
                        }}
                    />

                    <CreateTeamModal
                        isOpen={createTeamOpen}
                        setIsOpen={setIsCreateTeamOpen}
                        reloadTeams={async () => { 
                            const reloadTeams = await GetTeams();

                            setTeams(reloadTeams);
                        } }
                    />

                </ResizablePanel>

                <ResizableHandle className="bg-neutral-900" />

                <ResizablePanel defaultSize={90}>

                    <div className="top-0 z-20 p-4 h-24 justify-between">
                        <h1 className="font-semibold text-neutral-50 text-xl p-2 transition hover:[text-shadow:0_0_15px_#48abe0]">
                            {selectTeamVaultId ? `${selectedTeamName}'s Team Vault` : "Dashboard"}
                        </h1>
                    </div>

                    <div className="flex-1 p-4 max-h-[80vh] overflow-y-auto">

                        {inviteOverview ? (
                            <AccountPendingInvitesSegment email={email} />
                        ) : selectTeamVaultId ? (
                            <TeamVaultDashboard teamId={selectTeamVaultId} />
                        ) : (
                            <VaultDashboard
                                vaults={vaults}
                                reloadVaults={async () => {
                                    const reload = await GetVaultsAndKeys();
                                    setVaults(reload);
                                }}
                            />
                        )}


                    </div>

                </ResizablePanel>
            </ResizablePanelGroup>
        </div>
    );





}

export default HomePage;