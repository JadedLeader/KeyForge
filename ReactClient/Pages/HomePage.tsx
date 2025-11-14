import  { useState, useEffect } from "react"
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
    Sheet,
    SheetContent,
    SheetDescription,
    SheetHeader,
    SheetTitle,
    SheetTrigger,
} from "@/components/ui/sheet"
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover"
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
    flexRender,
    getCoreRowModel,
    getFilteredRowModel,
    getPaginationRowModel,
    getSortedRowModel,
    useReactTable,
} from "@tanstack/react-table"
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
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
import {
    Collapsible,
    CollapsibleContent,
    CollapsibleTrigger,
} from "@/components/ui/collapsible"
import { ChevronRight, MoreVertical, Trash2, Edit, Eye, EyeOff } from "lucide-react";

interface SilentTokenCycleRequest { 

}


interface SilentTokenCycleResponse { 
    refreshedToken: string; 
    successful: boolean;
}

interface CreateVaultRequest { 

    vaultName: string; 
    vaultType: string;
}

interface CreateVaultResponse { 
    
}

interface DecryptKeyRequest { 

    encryptedVaultkey: string; 
    vaultId: string;
}

interface DecryptKeyResponse { 
    decryptedVaultKey: string;
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

async function DecryptVaultKey(encryptedKey: string, vaultId: string): Promise<DecryptKeyResponse> {

    const buildingDecryptkeyRequest = BuildDecrpytKey(encryptedKey, vaultId);

    const decryptKeysResponse = await fetch("/VaultKeys/DecryptVaultKey", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        body: JSON.stringify(buildingDecryptkeyRequest)
    });

    if (!decryptKeysResponse.ok) {

        const errorText = await decryptKeysResponse.text();

        throw new Error(errorText);

    }

    const data = await decryptKeysResponse.json();

    const response = BuildDecryptKeyResponse(data.decryptedVaultKey)

    console.log("decrypting key returned:", response);

    return response;



}

function BuildDecryptKeyResponse(response: string): DecryptKeyResponse {


    const create: DecryptKeyResponse = {
        decryptedVaultKey: response,
    };

    return create;
}

function BuildDecrpytKey(encryptedKey: string, vaultId: string): DecryptKeyRequest {

    const createNewDecryptKey: DecryptKeyRequest = {
        encryptedVaultkey: encryptedKey,
        vaultId: vaultId

    };

    return createNewDecryptKey;

}



export function ButtonOutline() {
    return <Button variant="outline">Outline</Button>
}


export function VaultDashboard({ vaults }: {vaults : Vault[] }) {

    const [decryptedKey, setDecryptedKey] = useState<Record<string, string>>({});

    const handleRevealKey = async (encryptedKey: string, vaultId: string, vaultKeyId: string) => {


        const response = await DecryptVaultKey(encryptedKey, vaultId);

        //not sure what this does, revisit
            setDecryptedKey(prev => ({
                ...prev,
                [vaultKeyId]: response.decryptedVaultKey,
            }));
        
    };
    
    return (
        <div className="flex flex-wrap gap-4">

            {vaults.map((vault) => (

                <Card key={vault.vaultId} className="bg-zinc-950 text-white border-blue-600 w-80">
                    <CardHeader>
                        <CardTitle className="flex justify-between">
                            Vault Name: {vault.vaultName}

                            <DropdownMenu >
                                <DropdownMenuTrigger>

                                    <MoreVertical />


                                </DropdownMenuTrigger>

                                <DropdownMenuContent className="text-white bg-transparent border-0">

                                    <DropdownMenuItem>
                                        Edit
                                    </DropdownMenuItem>
                                    <DropdownMenuItem >
                                        Delete
                                    </DropdownMenuItem>
                                    <DropdownMenuItem>Expand</DropdownMenuItem>

                                </DropdownMenuContent>

                            </DropdownMenu>
                        </CardTitle>
               
                    </CardHeader>


                    <CardContent className="space-y-2">
                        {vault.keys.map((key) => (
                            <div key={key.vaultKeyId}>

                                <div className="flex justify-between">
                                <FieldLabel className="font-semibold text-white">Key Name:</FieldLabel>
                                <p className="text-gray-300 text-xs break-all">{key.keyName}</p>
                                </div>

                                
                                <FieldLabel className="text-white font-semibold">Vault Key</FieldLabel>

                                <div className="flex justify-between"> 
                                    <p className="text-gray-300 text-xs break-all">{decryptedKey[key.vaultKeyId] ?? key.hashedVaultKey} </p>
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

export function HomePage() {

    const [vaults, setVaults] = useState<Vault[]>([]);
    const [shortLivedToken, setShortLivedToken] = useState("");
    const [vaultName, setVaultName] = useState("");
    const [vaultType, setVaultType] = useState("");
    const [vaultKeyName, setVaultKeyName] = useState("");
    const [vaultKey, setVaultKey] = useState("");
    const [sidePanelOpen, setSidePanelOpen] = useState(true);
    const [isCreateVaultDialogOpen, setIsCreateVaultDialogOpen] = useState(false);
    const [eyeOpen, setEyeOpen] = useState(false)

    useEffect(() => {

        const fetchToken = async () => {

            const shortLivedToken = await SilentTokenRefresh();

            setShortLivedToken(shortLivedToken.refreshedToken);

            await GetVaultsAndKeys();

        }; 

        fetchToken();

        
        

    }, []);

    function HandleVaultNameChanged(e: React.ChangeEvent<HTMLInputElement>) { 

        setVaultName(e.target.value);
    }

    function HandleVaultTypeChanged(vaultType: string) { 
        setVaultType(vaultType);
    }

    async function SilentTokenRefresh(): Promise<SilentTokenCycleResponse> { 

        try {
            const buildingRequest = BuildSilentCycleRequest();

            const silentTokenResponse = await fetch("/Auth/SilentShortLivedTokenRefresh", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                body: JSON.stringify(buildingRequest),
            });

            if (!silentTokenResponse.ok) {
                const errorText = await silentTokenResponse.text();
                console.error("Silent token refresh failed:", errorText);
                return null; 
            }

            return await silentTokenResponse.json();
        } catch (err) {
            console.error("Silent token refresh threw an exception:", err);
            return null; 
        }

    }

    async function GetVaultsAndKeys(): Promise<void> { 


        try {


            const getVaults = await fetch("VaultKeys/GetAllVaultsWithKeys", {
                method: "GET",
                credentials: "include",

            });

            if (!getVaults) {

                throw new Error("Failed To Fetch Vaults")

            }

            const data: Vault[] = await getVaults.json();
            setVaults(data);
        } 
        catch (err) {
            console.error("get vaults and keys threw an exception", err);
            return null;
        }


    }
    

    function BuildSilentCycleRequest(): SilentTokenCycleRequest { 


        const newCycleRequest: SilentTokenCycleRequest = {

        }; 

        return newCycleRequest;
    }

    async function CreateNewVault(): Promise<CreateVaultResponse> { 

        const buildingCreateVaultRequest = BuildCreateVaultRequest(vaultName, vaultType); 

        const createNewVaultResponse = await fetch("/Vault/CreateVault", {
            method: "POST",
            headers: {
                "Content-type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify(buildingCreateVaultRequest)
        }); 

        if (!createNewVaultResponse.ok) { 

            const errorText = await createNewVaultResponse.text();

            throw new Error(errorText);
        }

        console.log("Submitting vault:", vaultName, vaultType);

        return createNewVaultResponse;

    }

    function BuildCreateVaultRequest(vaultNameGiven: string, vaultTypeGiven: string) : CreateVaultRequest { 

        const createNewVaultRequest: CreateVaultRequest = {

            vaultName: vaultNameGiven,
            vaultType: vaultTypeGiven
        };

        return createNewVaultRequest;

    }


    

    return (

        <div className="dark min-h-screen flex relative">
            
            <Sheet open={sidePanelOpen} onOpenChange={setSidePanelOpen} modal={false}>
                <SheetTrigger className="absolute top-4 left-4 bg-blue-600 text-white p-2 rounded z-50">
                    <ChevronRight size={20} />
                </SheetTrigger>
                <SheetContent
                    side="left"
                    className="w-64 bg-zinc-950 text-white border-r border-blue-600 flex flex-col"
                >
                    <SheetHeader>
                        <SheetTitle>Menu</SheetTitle>
                        <SheetDescription>
                            This action cannot be undone.
                        </SheetDescription>

                        <DropdownMenu>
                            <DropdownMenuTrigger className="justify-start bg-transparent hover:bg-transparent text-blue-500 hover:underline text-left">
                                Vaults
                            </DropdownMenuTrigger>
                            <DropdownMenuContent className="w-56" align="start">
                                <DropdownMenuSeparator />
                                <DropdownMenuItem
                                    onClick={() => setIsCreateVaultDialogOpen(true)}
                                    className="bg-transparent text-white hover:underline"
                                >
                                    Create Vault
                                </DropdownMenuItem>
                                <DropdownMenuItem className="bg-transparent text-white hover:underline">
                                    Delete Vault
                                </DropdownMenuItem>
                                <DropdownMenuItem className="bg-transparent text-white">
                                    Edit Vault
                                </DropdownMenuItem>
                                <DropdownMenuSeparator />
                            </DropdownMenuContent>
                        </DropdownMenu>
                    </SheetHeader>
                </SheetContent>
            </Sheet>

     
            

                
            <div className={`flex-1 flex flex-col transition-transform duration-300 ${sidePanelOpen ? 'ml-64' : 'ml-0'
                }`}>
                   
                <div className="w-full h-14 flex items-center top-4">
                    <h1 className={`text-white text-xl font-semibold transition-all duration-500
                ${sidePanelOpen ? 'ml-8' : 'ml-12'}`}>Vaults</h1>
                    </div>

                    
                    <div className="flex-1 p-4">
                      
                    <VaultDashboard vaults={vaults} />
                    </div>
                </div>
            </div>
        
    );





}

export default HomePage;