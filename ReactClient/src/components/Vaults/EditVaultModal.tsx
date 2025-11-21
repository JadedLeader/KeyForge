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
import { UpdateVaultName } from "@/components/api/Vault"
import { UpdateVaultKeyAndKeyName, GetVaultWithAllDetails } from "@/components/api/VaultKeys"
import { toast } from "sonner"


type VaultKey = {
    vaultKeyId: string;
    keyName: string;
    hashedVaultKey: string;
    dateTimeVaultKeyCreated: string;
}

interface EditVaultModalProps { 


    vaultId: string;
    reloadVaults: () => void;
    isOpen: boolean;
    setIsOpen: React.Dispatch<React.SetStateAction<boolean >>;
    currentVaultName: string;

}



interface UpdateVaultKeysKeyNameAndVaultNameResponse { 
    vaultId: string; 
    newKeyName: string; 
    newVaultKey: string; 
    updatedVaultName: string;
    sucessful: boolean;

}

export function EditVaultModal({vaultId, reloadVaults, isOpen, setIsOpen, currentVaultName} : EditVaultModalProps) {


    const [vaultName, setVaultName] = useState("");
    const [newKeyName, setNewKeyName] = useState("");
    const [newVaultKey, setNewVaultKey] = useState("");

    const [vaultKeyId, setVaultKeyId] = useState("");
    const [oldKeyName, setOldKeyName] = useState("");
    const [oldVaultKey, setOldVaultKey] = useState("");

    useEffect(() => { 

        const loadOnMount = async () => { 

            const specificVault = await GetVaultWithAllDetails(vaultId);

            {specificVault.vaultKeys.map((keys) => { 

                setOldKeyName(keys.keyName);
                setOldVaultKey(keys.hashedVaultKey);
                setVaultKeyId(keys.vaultKeyId);

                console.log(vaultKeyId);

            })}

        }

        loadOnMount();


    },[])

    function SetKeyName(newKeyName: React.ChangeEvent<HTMLInputElement>) { 

        setNewKeyName(newKeyName.target.value)

    }

    function SetVaultKey(newVaultKey: React.ChangeEvent<HTMLInputElement>) { 
        setNewVaultKey(newVaultKey.target.value);
    }

    function SetVault(vaultName: React.ChangeEvent<HTMLInputElement>) { 
        setVaultName(vaultName.target.value);
    }

    function sleep(ms: number) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    async function UpdateVaultKeysKeyNameAndVaultName(vaultKeyId: string): Promise<UpdateVaultKeysKeyNameAndVaultNameResponse> { 


        const updatedVaultKeyAndKeyName = await UpdateVaultKeyAndKeyName(vaultKeyId, newKeyName, newVaultKey);

        console.log("update vault key and key name", updatedVaultKeyAndKeyName);

        const updatedVaultName = await UpdateVaultName(vaultName, vaultId);

        console.log("update vault name", updatedVaultName);

        const response: UpdateVaultKeysKeyNameAndVaultNameResponse = {
            vaultId: vaultId,
            newKeyName: updatedVaultKeyAndKeyName.newKeyName,
            newVaultKey: updatedVaultKeyAndKeyName.newEncryptedKey,
            updatedVaultName: updatedVaultName.updatedVaultName,
            sucessful: updatedVaultName.sucessful
        }; 

        return response;

    }


  return (

      <div>

          <Dialog open={isOpen} onOpenChange={setIsOpen} >

              <DialogContent className="expand-label-theme expand-label-theme-hover overflow-y-auto">

                  <DialogHeader>

                      <DialogTitle>Vault Name: {currentVaultName}</DialogTitle>

                      <DialogDescription>

                          Current details have already been filled

                      </DialogDescription>
                    
                  </DialogHeader>

                  <Label className="title-text"> Change Vault Name:</Label>
                  <Input value={vaultName} onChange={SetVault} type="text" placeholder={currentVaultName}></Input>
      
                  <Label className="title-text">Change Key Name:</Label>
                  <Input value={newKeyName} onChange={SetKeyName} type="text" placeholder={oldKeyName} ></Input>

                  <Label className="title-text">Change Key:</Label>
                  <Input value={newVaultKey} onChange={SetVaultKey} type="text" placeholder={oldVaultKey} ></Input>
            

                  <DialogFooter>

                      <DialogClose asChild>

                          <Button className="bg-blue-600 text-white hover::underline">Close</Button>

                      </DialogClose>

                      <Button className="bg-blue-600 text-white hover::underline" onClick={async () => {

                          const updateVaultDetails = await UpdateVaultKeysKeyNameAndVaultName(vaultKeyId);

                          if (updateVaultDetails.sucessful) {

                              toast.success("Vault updated successfully!");

                              await sleep(1000);

                              reloadVaults();

                              setIsOpen(false);

                              
                          }
                          else { 
                              toast.error("Something went wrong when trying to update your vault.");
                          }

                      }}> Update Vault</Button>

                  </DialogFooter>

              </DialogContent>

          </Dialog>

      </div>
       
  );


}

export default EditVaultModal;