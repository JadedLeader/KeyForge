interface RemoveAllVaultKeysFromVaultRequest {
    VaultId: string;
}

interface RemoveAllVaultKeysFromVaultResponse {
    vaultId: string;
    success: boolean;
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
    success: boolean;

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

interface BuildUpdateVaultKeyAndKeyNameRequest {
    vaultKeyId: string;
    newKeyName: string;
    newVaultKey: string;
}

interface BuildUpdateVaultKeyAndKeyNameResponse {
    newEncryptedKey: string;
    newKeyName: string;
    success: boolean;
}
interface GetVaultWithAllDetailsRequest {
    vaultId: string;
}

interface GetVaultWithAllDetailsResponse {

    vaultName: string;
    success: boolean;
    vaultKeys: VaultKey[];


}

function BuildDeleteVaultWithAllKeysRequest(vaultId: string): RemoveAllVaultKeysFromVaultRequest {

    const buildingRequest: RemoveAllVaultKeysFromVaultRequest = {
        VaultId: vaultId
    };

    console.log("vault id being pinged:", buildingRequest.VaultId);

    return buildingRequest;

}

function BuildDeleteVaultWithAllVaultKeysResponse(vaultId: string, success: boolean): RemoveAllVaultKeysFromVaultResponse {

    const buildingResponse: RemoveAllVaultKeysFromVaultResponse = {

        vaultId: vaultId,
        success: success
    };

    return buildingResponse;

}

export async function DeleteAllKeysFromVault(vaultId: string): Promise<RemoveAllVaultKeysFromVaultResponse> {


    const buildingRequestBody = BuildDeleteVaultWithAllKeysRequest(vaultId);


    const deleteVaultKeysCall = await fetch("/VaultKeys/CascadeDeleteVaultKeysFromVault", {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(buildingRequestBody),
    });

    console.log(deleteVaultKeysCall);


    if (!deleteVaultKeysCall.ok) {

        const errorText = await deleteVaultKeysCall.text();

        throw new Error(errorText);

    }

    const jsonBody = await deleteVaultKeysCall.json();

    const buildingPromise = BuildDeleteVaultWithAllVaultKeysResponse(jsonBody.vaultId, jsonBody.sucess);

    console.log("Deleting vault with keys", jsonBody);

    return buildingPromise;
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

export async function DecryptVaultKey(encryptedKey: string, vaultId: string): Promise<DecryptKeyResponse> {

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

function BuildCreateVaultKeyRequest(keyName: string, passwordToEncrypt: string, vaultId: string): CreateVaultKeyRequest {

    const buildingVaultKey: CreateVaultKeyRequest = {

        keyName: keyName,
        passwordToEncrypt: passwordToEncrypt,
        vaultId: vaultId
    }

    return buildingVaultKey;

}

function BuildCreateVaultKeyResponse(vaultName: string, success: boolean): CreateVaultKeyResponse {

    const buildingVaultKeyResponse: CreateVaultKeyResponse = {


        vaultName: vaultName,
        success: success

    };

    return buildingVaultKeyResponse;

} 

export async function CreateNewVaultKey(keyName: string, keyPassword: string, vaultId: string): Promise<CreateVaultKeyResponse> {

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

    console.log("build keys", returnJson);

    const buildingResponse = BuildCreateVaultKeyResponse(returnJson.vaultName, returnJson.success);

    return buildingResponse;


}

export async function GetVaultsAndKeys(): Promise<Vault[]> {


    try {


        const getVaults = await fetch("VaultKeys/GetAllVaultsWithKeys", {
            method: "GET",
            credentials: "include",

        });

        if (!getVaults) {

            throw new Error("Failed To Fetch Vaults")

        }

        const data: Vault[] = await getVaults.json();

        return data;
    }
    catch (err) {
        console.error("get vaults and keys threw an exception", err);
        throw err;
    }

}

function CreateUpdateVaultBody(vaultKeyId: string, newKeyName: string, newVaultKey: string): BuildUpdateVaultKeyAndKeyNameRequest {

    const buildingRequestBody: BuildUpdateVaultKeyAndKeyNameRequest = {
        vaultKeyId: vaultKeyId,
        newVaultKey: newVaultKey,
        newKeyName: newKeyName
    };

    return buildingRequestBody;

}

function CreateUpdateVaultResponse(newEncryptedKey: string, newKeyName: string, success: boolean): BuildUpdateVaultKeyAndKeyNameResponse {

    const buildingResponseBody: BuildUpdateVaultKeyAndKeyNameResponse = {

        newKeyName: newKeyName,
        newEncryptedKey: newEncryptedKey,
        success: success

    };

    return buildingResponseBody;

}

export async function UpdateVaultKeyAndKeyName(vaultKeyId: string, newKeyName: string, newVaultKey: string): Promise<BuildUpdateVaultKeyAndKeyNameResponse> {

    const buildingRequestBody = CreateUpdateVaultBody(vaultKeyId, newKeyName, newVaultKey);

    const fetchingEndpoint = await fetch("/VaultKeys/UpdateVaultKeyAndKeyName", {
        method: "PUT",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildingRequestBody)
    });


    if (!fetchingEndpoint.ok) {

        const errorText = await fetchingEndpoint.text();

        throw new Error(errorText);

    }

    const responseBody = await fetchingEndpoint.json();

    const buildResponse = CreateUpdateVaultResponse(responseBody.newEncryptedKey, responseBody.newKeyName, responseBody.success);

    return buildResponse;

}

function BuildGetVaultWithAllDetailsResponse(success: boolean, vaultName: string, vaultKeys: VaultKey[]): GetVaultWithAllDetailsResponse {

    const response: GetVaultWithAllDetailsResponse = {
        success: success, 
        vaultKeys: vaultKeys, 
        vaultName: vaultName

    };

    return response;

}

function BuildGetVaultWithAllDetailsRequest(vaultId: string): GetVaultWithAllDetailsRequest {

    const buildingRequest: GetVaultWithAllDetailsRequest = {
        vaultId: vaultId

    };

    return buildingRequest;

}


export async function GetVaultWithAllDetails(vaultId: string): Promise<GetVaultWithAllDetailsResponse> {


    const buildingRequest = BuildGetVaultWithAllDetailsRequest(vaultId);

    const getVault = await fetch("/VaultKeys/GetVaultWithAllDetails", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildingRequest)
    });

    if (!getVault.ok) {

        const errorText = await getVault.text();

        throw new Error(errorText);
    }

    const jsonBody = await getVault.json();

    const buildingResponse = BuildGetVaultWithAllDetailsResponse(jsonBody.success, jsonBody.vaultName, jsonBody.vaultKeys);

    return buildingResponse;
}