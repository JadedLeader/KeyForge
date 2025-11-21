interface DeleteVaultRequest {
    vaultId: string;
}

interface DeleteVaultResponse {
    vaultId: string;
    accountId: string;
    sucessful: boolean;
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

interface BuildUpdateVaultNameRequest {
    vaultName: string;
    vaultId: string;
}

interface BuildUpdateVaultNameResponse {
    vaultId: string;
    updatedVaultName: string;
    sucessful: boolean;
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

export async function DeleteVault(vaultId: string): Promise<DeleteVaultResponse> {

    const buildingRequestBody = BuildDeleteVaultRequest(vaultId);

    console.log("hit delete vault", buildingRequestBody)

    const deleteVaultRequest = await fetch("/Vault/DeleteVault", {
        method: "DELETE",
        headers: {
            "content-type": "application/json"
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

    console.log("deleting vault", jsonBody);

    return buildingPromise;


}

function BuildCreateVaultRequest(vaultNameGiven: string, vaultTypeGiven: string): CreateVaultRequest {

    const createNewVaultRequest: CreateVaultRequest = {

        vaultName: vaultNameGiven,
        vaultType: vaultTypeGiven
    };

    return createNewVaultRequest;

}

function BuildCreateVaultResponse(vaultId: string, vaultName: string, sucessful: boolean): CreateVaultResponse {

    const newVaultResponse: CreateVaultResponse = {
        vaultId: vaultId,
        vaultName: vaultName,
        sucessful: sucessful

    };

    return newVaultResponse;

}

export async function CreateNewVault(vaultName: string, vaultType: string): Promise<CreateVaultResponse> {

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

function CreateUpdateVaultNameBody(vaultName: string, vaultId: string): BuildUpdateVaultNameRequest {

    const buildingRequest: BuildUpdateVaultNameRequest = {

        vaultName: vaultName,
        vaultId: vaultId

    };


    return buildingRequest;

}

function CreateUpdateVaultNameResponse(vaultId: string, updatedVaultName: string, sucessful: boolean): BuildUpdateVaultNameResponse {

    const buildingResponse: BuildUpdateVaultNameResponse = {

        vaultId: vaultId,
        updatedVaultName: updatedVaultName,
        sucessful: sucessful

    };

    return buildingResponse;

}

export async function UpdateVaultName(vaultName: string, vaultId: string): Promise<BuildUpdateVaultNameResponse> {

    const buildRequestBody = CreateUpdateVaultNameBody(vaultName, vaultId);

    const updateResponse = await fetch("Vault/UpdateVault", {
        method: "PUT",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildRequestBody)

    });

    if (!updateResponse.ok) {

        const errorText = await updateResponse.text();

        throw new Error(errorText);

    }

    const jsonBody = await updateResponse.json();

    const response = CreateUpdateVaultNameResponse(jsonBody.vaultId, jsonBody.updatedVaultName, jsonBody.sucessful);

    return response;

}

