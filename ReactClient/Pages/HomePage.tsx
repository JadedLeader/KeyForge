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
import { Separator } from "../src/components/ui/separator"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { toast } from "sonner"
import { EditVaultModal } from "@/components/Vaults/EditVaultModal"
import { GetUserAccountDetails } from "@/components/api/Account"
import { SilentTokenRefresh } from "@/components/api/Auth"
import { DeleteVault, CreateNewVault } from "@/components/api/Vault"
import { DeleteAllKeysFromVault, DecryptVaultKey, CreateNewVaultKey, GetVaultsAndKeys, GetVaultWithAllDetails } from "@/components/api/VaultKeys"


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
}

export function BuildCeateVaultModal({dialogOpen, setDialogOpen, reloadVaults } : BuildCreateVaultModalProps)  { 


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

    

    async function CreateVaultWithKey(vaultName: string, vaultType: string, keyName: string, keyPassword: string): Promise<CreateVaultWithKeysResponse> { 

        const buildVault = await CreateNewVault(vaultName, vaultType);

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

                    <Label>Vault Type</Label>

                    <div className={dropDownOpen ? "mb-20" : "mb-4"} >
                        <Select value={vaultType} onValueChange={HandleVaultTypeChanged} onOpenChange={() => HandleDropdownOpen(true)} >
                            <SelectTrigger className="border-0 hover:underline" >
                                <SelectValue placeholder="Select vault type" className="Textplaceholder::placeholder text-white bg-transparent" />
                            </SelectTrigger>
                            <SelectContent className="bg-transparent border-0 hover:underline">
                                <SelectItem className="text-white bg-transparent hover::underline" value="Personal">Personal</SelectItem>
                                <SelectItem className="text-white bg-transparent hover::underline" value="Team">Team</SelectItem>
                            </SelectContent>
                        </Select>
                    </div>

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

                            const result = await CreateVaultWithKey(vaultName, vaultType, keyName, keyPassword);
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


                            } else {
                                console.log("Vault did not create");
                                toast.error("Oops, creating a vault didn’t work.");
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

export function BuildAvatarAndUsernameSideBarSegment() { 

    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");

    useEffect(() => {

        const buildAvatarData = async () => {

            
            const response = await GetUserAccountDetails();

            setUsername(response.username);
            setEmail(response.email);
        };

        buildAvatarData();

    }, []);

   

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

   
    useEffect(() => {

        const fetchToken = async () => {

            const shortLivedToken = await SilentTokenRefresh();

            setShortLivedToken(shortLivedToken.refreshedToken);

            const vaults = await GetVaultsAndKeys();

            setVaults(vaults);

            

        }; 

        fetchToken();




    }, []);


    return (

        <div className="flex relative h-screen">

            <ResizablePanelGroup direction="horizontal" className="h-full">

                <ResizablePanel defaultSize={10} className="sidepanel-theme sidepanel-theme-hover ">

                    <div className="flex flex-col h-full">

                        <div>

                            <BuildAvatarAndUsernameSideBarSegment />

                            <Separator className="my-4 bg-[hsl(210,12%,12%)]" orientation="horizontal" />

                            <div className={`${vaultDropdownOpen ? "mb-40" : "mb-4"}`}> 

                            <DropdownMenu onOpenChange={setVaultDropDownOpen}>
                                <DropdownMenuTrigger className="justify-start text-blue-500 hover:underline text-left text-white px-4">
                                    Vaults
                                </DropdownMenuTrigger>
                                <DropdownMenuContent className="w-56 px-4" align="start">
                                    <DropdownMenuSeparator className="border-neutral-700" />
                                    <DropdownMenuItem
                                        onClick={() => setIsCreateVaultDialogOpen(true)}
                                        className="bg-transparent text-white hover:underline"
                                    >
                                        Create Vault
                                    </DropdownMenuItem>
                                    <DropdownMenuSeparator />
                                </DropdownMenuContent>
                                </DropdownMenu>

                            </div>

                            <Separator className="my-4 bg-[hsl(210,12%,12%)]" orientation="horizontal" />

                            <DropdownMenu>
                                <DropdownMenuTrigger className="justify-start text-blue-500 hover:underline text-left text-white px-4">
                                    Teams
                                </DropdownMenuTrigger>
                                <DropdownMenuContent className="w-56 px-4" align="start">

                                    <DropdownMenuItem className="bg-transparent text-white hover:underline">
                                        Create Team
                                    </DropdownMenuItem>
                                    <DropdownMenuSeparator />
                                    <DropdownMenuItem className="bg-transparent text-white hover:underline">
                                        Add Team Member
                                    </DropdownMenuItem>
                                    <DropdownMenuSeparator />
                                    <DropdownMenuItem className="bg-transparent text-white hover:underline">
                                        Team Invitations
                                    </DropdownMenuItem>
                                    <DropdownMenuSeparator />
                                    <DropdownMenuItem className="bg-transparent text-white hover:underline">
                                        Remove Team Member
                                    </DropdownMenuItem>

                                </DropdownMenuContent>

                            </DropdownMenu>

                            <Separator className="my-4 bg-[hsl(210,12%,12%)]" orientation="horizontal" />

                        </div>

                    </div>

                    <BuildCeateVaultModal dialogOpen={isCreateVaultDialogOpen} setDialogOpen={setIsCreateVaultDialogOpen} reloadVaults={async () =>
                    {

                        const getVaults = await GetVaultsAndKeys();

                        setVaults(getVaults);
                    } 
                    }  />

                </ResizablePanel >
      
                <ResizableHandle className="bg-neutral-900" />

                <ResizablePanel defaultSize={90} >

                    <div className="top-0 z-20 p-4 h-24 justify-between">
     
                        <h1 className="font-semibold text-neutral-50 text-xl p-2 transition hover:[text-shadow:0_0_15px_#48abe0]">Dashboard</h1>

                    </div>

                    <div >

                        <div className="flex-1 p-4 max-h-[80vh] overflow-y-auto">
                            <VaultDashboard vaults={vaults} reloadVaults={async () =>
                            {
                                const reload = await GetVaultsAndKeys(); 

                                setVaults(reload);
                            }} />
                        </div>
                    </div>

                </ResizablePanel>
               

            </ResizablePanelGroup >
        </div>
            
        
    );





}

export default HomePage;