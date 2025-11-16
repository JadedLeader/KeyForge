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
import { ChevronRight, MoreVertical, Trash2, Edit, Eye, EyeOff } from "lucide-react" 
import {
    ResizableHandle,
    ResizablePanel,
    ResizablePanelGroup,
} from "@/components/ui/resizable"

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
    vaultId: string;
    vaultName: string;
    sucessful: boolean;
}

interface DecryptKeyRequest { 

    encryptedVaultkey: string; 
    vaultId: string;
}

interface DecryptKeyResponse { 
    decryptedVaultKey: string;
}

interface CreateVaultKeyRequest { 
    vaultId: string;
    keyName: string; 
    passwordToEncrypt: string;
}

interface CreateVaultKeyResponse { 

    vaultName: string; 
    sucess: boolean;

}

interface CreateVaultWithKeysResponse { 

    vaultId: string; 
    vaultName: string;

}

interface RemoveAllVaultKeysFromVaultRequest { 
    vaultId: string;
}

interface RemoveAllVaultKeysFromVaultResponse { 
    vaultId: string; 
    success: boolean;
}

interface DeleteVaultRequest { 
    vaultId: string;
}

interface DeleteVaultResponse { 
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

    function BuildDeleteVaultWithAllKeysRequest(vaultId: string): RemoveAllVaultKeysFromVaultRequest { 

        const buildingRequest: RemoveAllVaultKeysFromVaultRequest = {
            vaultId: vaultId
        }; 

        return buildingRequest;

    }

    function BuildDeleteVaultWithAllVaultKeysResponse(vaultId: string, success: boolean): RemoveAllVaultKeysFromVaultResponse { 

        const buildingResponse: RemoveAllVaultKeysFromVaultResponse = {

            vaultId: vaultId,
            success: success
        }; 

        return buildingResponse;

    }

    async function DeleteVaultWithAllKeys(vaultId : string) : Promise<RemoveAllVaultKeysFromVaultResponse> { 


        const buildingRequestBody = BuildDeleteVaultWithAllKeysRequest(vaultId);

        const deleteVaultKeysCall = await fetch("/VaultKeys/RemoveAllVaultKeysFromVault", {
            method: "DELETE",
            headers: {
                "content-type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify(buildingRequestBody)
        });

        if (!deleteVaultKeysCall.ok) { 

            const errorText = await deleteVaultKeysCall.text(); 

            throw new Error(errorText);

        }

        const jsonBody = await deleteVaultKeysCall.json();

        const buildingPromise = BuildDeleteVaultWithAllVaultKeysResponse(jsonBody.vaultId, jsonBody.sucess);

        console.log("Deleting vault with keys", jsonBody);

        return buildingPromise;
    }

    function BuildDeleteVaultRequest(vaultId: string): DeleteVaultRequest { 

        const buildingRequest: DeleteVaultRequest = {
            vaultId: vaultId
        }; 

        return buildingRequest;

    }

    function BuildDeleteVaultResponse(vaultId: string, accountId: string, sucessful: boolean): DeleteVaultResponse { 

        const buildingResponse: DeleteVaultResponse = {
            accountId: accountId,
            vaultId: vaultId,
            sucessful: sucessful
        }; 

        return buildingResponse;

    }

    async function DeleteVault(vaultId: string) : Promise<DeleteVaultResponse> { 

        const buildingRequestBody = BuildDeleteVaultRequest(vaultId); 

        const deleteVaultRequest = await fetch("/Vault/DeleteVault", {
            method: "DELETE",
            headers: {
                "content-type": "application.json"
            },
            credentials: "include",
            body: JSON.stringify(buildingRequestBody)
        });

        if (!deleteVaultRequest.ok) { 

            const errorText = await deleteVaultRequest.text(); 

            throw new Error(errorText);

        }

        const jsonBody = await deleteVaultRequest.json();

        const buildingPromise = BuildDeleteVaultResponse(jsonBody.vaultId, jsonBody.accountId, jsonBody.sucessful);

        return buildingPromise; 


    }


    
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
                                    <DropdownMenuItem  >
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

interface BuildCreateVaultModalProps { 
    dialogOpen: boolean; 
    setDialogOpen: React.Dispatch<React.SetStateAction<boolean>>;
}

export function BuildCeateVaultModal({dialogOpen, setDialogOpen } : BuildCreateVaultModalProps)  { 


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

    function BuildCreateVaultRequest(vaultNameGiven: string, vaultTypeGiven: string): CreateVaultRequest {

        const createNewVaultRequest: CreateVaultRequest = {

            vaultName: vaultNameGiven,
            vaultType: vaultTypeGiven
        };

        return createNewVaultRequest;

    }

    function BuildCreateVaultResponse(vaultId: string, vaultName: string, sucessful: boolean) : CreateVaultResponse { 

        const newVaultResponse: CreateVaultResponse = {
            vaultId: vaultId, 
            vaultName: vaultName, 
            sucessful: sucessful

        }; 

        return newVaultResponse;

    }
    function BuildCreateVaultKeyRequest(keyName: string, passwordToEncrypt: string, vaultId: string): CreateVaultKeyRequest { 

        const buildingVaultKey: CreateVaultKeyRequest = { 

            keyName: keyName, 
            passwordToEncrypt: passwordToEncrypt, 
            vaultId : vaultId
        }

        return buildingVaultKey;

    }

    function BuildCreateVaultKeyResponse(vaultName: string, sucessful: boolean): CreateVaultKeyResponse { 

        const buildingVaultKeyResponse: CreateVaultKeyResponse = {


            vaultName: vaultName,
            sucess: sucessful

        }; 

        return buildingVaultKeyResponse;

    } 

    function BuildVaultWithKeysResponse(vaultId: string, vaultName: string): CreateVaultWithKeysResponse { 

        const buildingVaultWithKeysResponse: CreateVaultWithKeysResponse = {

            vaultId: vaultId,
            vaultName: vaultName


        }; 

        return buildingVaultWithKeysResponse;

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

        

        const returnJson = await createNewVaultResponse.json();

        console.log("Submitting vault:", vaultName, vaultType, returnJson.vaultId);

        const built = BuildCreateVaultResponse(returnJson.vaultId, returnJson.vaultName, returnJson.sucessful);

        return built;

    }

    async function CreateNewVaultKey(vaultId: string): Promise<CreateVaultKeyResponse> { 

        const buildVaultKeyRequest = BuildCreateVaultKeyRequest(keyName, keyPassword, vaultId);

        console.log("sending vault key request", buildVaultKeyRequest);

        const fetchingApi = await fetch("/VaultKeys/CreateVaultKey", {

            method: "POST",
            headers: {
                "content-type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify(buildVaultKeyRequest)

        });


        if (!fetchingApi.ok) { 

            const errorText = await fetchingApi.text(); 

            throw new Error(errorText);
        }
            
        const returnJson = await fetchingApi.json();

        const buildingResponse = BuildCreateVaultKeyResponse(returnJson.vaultName, returnJson.sucessful);

        return buildingResponse;
        

    }

    async function CreateVaultWithKey(): Promise<CreateVaultWithKeysResponse> { 

        const buildVault = await CreateNewVault();

        const buildKeys = await CreateNewVaultKey(buildVault.vaultId);

        const buildingResponse = BuildVaultWithKeysResponse(buildVault.vaultId, buildKeys.vaultName); 

        return buildingResponse;

    }

    return (
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}  >
            <DialogContent className="bg-zinc-950 text-white border-blue-600 overflow-y-auto ">

                    <DialogHeader >

                        <DialogTitle >Create Vault</DialogTitle>

                    </DialogHeader>

                    <Label>Vault Name</Label>
                    <Input value={vaultName} onChange={HandleVaultNameChanged} />

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
                    <Input value={keyName} onChange={HandleKeyNameChanged} />

                    <Label>Password</Label>
                    <Input value={keyPassword} onChange={HandleKeyPasswordChanged} />


                    <DialogFooter >

                        <DialogClose asChild>
                            <Button className="bg-blue-600 text-white hover::underline" variant="outline" > Close </Button>
                        </DialogClose>

                        <Button className="bg-blue-600 text-white hover::underline" variant="outline" onClick={() => CreateVaultWithKey()}> Create Vault </Button>

                    </DialogFooter>

            </DialogContent>
        </Dialog>

    );
    


}

export function HomePage() {

    const [vaults, setVaults] = useState<Vault[]>([]);
    const [shortLivedToken, setShortLivedToken] = useState("");
    const [vaultName, setVaultName] = useState("");
    const [isCreateVaultDialogOpen, setIsCreateVaultDialogOpen] = useState(false);
    const [vaultKeyName, setVaultKeyName] = useState("");
    const [vaultKey, setVaultKey] = useState("");
    const [eyeOpen, setEyeOpen] = useState(false)

    useEffect(() => {

        const fetchToken = async () => {

            const shortLivedToken = await SilentTokenRefresh();

            setShortLivedToken(shortLivedToken.refreshedToken);

            await GetVaultsAndKeys();

        }; 

        fetchToken();


    }, []);

    

    async function SilentTokenRefresh(): Promise<SilentTokenCycleResponse> { 

        
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
                 
            }

            return await silentTokenResponse.json();
        

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
            
        }


    }
    

    function BuildSilentCycleRequest(): SilentTokenCycleRequest { 


        const newCycleRequest: SilentTokenCycleRequest = {

        }; 

        return newCycleRequest;
    }

    return (

        <div className="flex relative h-screen">

            <ResizablePanelGroup direction="horizontal" className="h-full">

                <ResizablePanel defaultSize={15} className="bg-zinc-950 border border-blue-600 flex flex-col ">

                    <div className="flex flex-col h-full">


                            <h1 className="text-white">Navigation</h1>

                            <DropdownMenu>
                                    <DropdownMenuTrigger className="justify-start bg-transparent hover:bg-transparent text-blue-500 hover:underline text-left text-white">
                                        Drop down for something
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

                    </div>

                    <BuildCeateVaultModal dialogOpen={isCreateVaultDialogOpen} setDialogOpen={setIsCreateVaultDialogOpen}  />

                </ResizablePanel >
      
                <ResizableHandle withHandle className="bg-blue-600" />

                <ResizablePanel defaultSize={85} >

                    <div className="p-4"> 

                        <h1 className="text-white justify-left p-4">Dashboard</h1>

                    </div>

                    <div >

                        <div className="flex-1 p-4 max-h-[80vh] overflow-y-auto">
                            <VaultDashboard vaults={vaults} />
                        </div>
                    </div>

                </ResizablePanel>
               

            </ResizablePanelGroup >
        </div>
            
        
    );





}

export default HomePage;