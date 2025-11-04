import { React, useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
import {
    Sheet,
    SheetContent,
    SheetDescription,
    SheetHeader,
    SheetTitle,
    SheetTrigger,
} from "@/components/ui/sheet";
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
    DialogFooter,
    DialogClose
} from "@/components/ui/dialog";
import "./HomePage.css"; 
import { ChevronRight } from "lucide-react";
import { Label } from "@/components/ui/label";

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


export function HomePage() {

    const [shortLivedToken, setShortLivedToken] = useState("");
    const [vaultName, setVaultName] = useState("");
    const [vaultType, setVaultType] = useState("");
    const [sidePanelOpen, setSidePanelOpen] = useState(true);

    useEffect(() => {

        const fetchToken = async () => {

            const shortLivedToken = await SilentTokenRefresh();

            setShortLivedToken(shortLivedToken.refreshedToken);
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
    

      <div className="dark"> 

          <Sheet open={sidePanelOpen} onOpenChange={setSidePanelOpen} className="sidemenu-dark">
              <SheetTrigger>
                  <div className="absolute top-4 left-4 bg-gray-700 text-white p-2 rounded cursor-pointer hover:bg-gray-600">
                      <ChevronRight size={20} />
                  </div>
              </SheetTrigger>
              <SheetContent side="left" className="sidemenu-dark sidemenuDescription-dark">
                  <SheetHeader>
                      <SheetTitle className="sidemenuDescription-dark">Menu</SheetTitle>
                      <SheetDescription className="sidemenuDescription-dark">
                          This action cannot be undone. This will permanently delete your account
                          and remove your data from our servers.
                      </SheetDescription>

                      <Dialog className="dialog-dark-form">
                          <form>

                              <DialogTrigger asChild>
                                  <Button className="bg-blue-600 text-white" variant="outline">Create Vault</Button>
                              </DialogTrigger>

                              <DialogContent className="dialog-dark">

                                  <form className="dialog-dark-form" onSubmit={CreateNewVault} > 

                                      <DialogHeader className="dialog-header">

                                        <DialogTitle className="dialog-title">Create Vault</DialogTitle>
                                      
                                      </DialogHeader>

                                  <div className="grid gap-3 space-y-2">
                                      <div className="space-y-2">
                                          <Label htmlFor="name-1">Vault Name</Label>
                                          <Input value={vaultName} onChange={HandleVaultNameChanged} />
                                  
                                          <Label htmlFor="vault-type">Vault Type</Label>

                                          <Select value={vaultType} onValueChange={HandleVaultTypeChanged}>
                                              <SelectTrigger>
                                                  <SelectValue placeholder="Select vault type" className="Textplaceholder::placeholder" />
                                              </SelectTrigger>
                                              <SelectContent>
                                                  <SelectItem value="Personal">Personal</SelectItem>
                                                  <SelectItem value="Team">Team</SelectItem>
                                              </SelectContent>
                                          </Select>

                                      </div>
                                  </div>

                                  <DialogFooter className="dialog-footer">

                                      <DialogClose asChild>
                                          <Button className="bg-blue-600 text-white" variant="outline">Cancel</Button>
                                      </DialogClose>

                                      <Button className="bg-blue-600 text-white" type="submit">Create Vault</Button>

                                      </DialogFooter>

                                  </form>
                              </DialogContent>
                          </form>
                      </Dialog>
                      


                  </SheetHeader>
              </SheetContent>
          </Sheet>
        

      </div>

    );





}

export default HomePage;