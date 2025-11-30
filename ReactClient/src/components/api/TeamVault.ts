interface CreateTeamVaultRequest { 
    teamId: string; 
    teamVaultDescription: string; 
    teamVaultName: string; 
    currentStatus: string;
}

interface CreateTeamVaultResponse { 
    teamId: string; 
    teamVaultId: string; 
    teamVaultDescription: string; 
    teamVaultName: string; 
    currentStatus: string; 
    success: boolean;
}

interface GetTeamsWithNoVaultsResponse { 

    teamsWithNoVaults: Team[];
    success: boolean;

}

type Team = { 
    id: string; 
    teamName: string;
}

interface DeleteTeamVaultRequest { 
    teamVaultId: string;
}

interface DeleteTeamVaultResponse { 
    teamVaultId: string;
    success: boolean;
}

interface UpdateTeamVaultRequest { 
    teamVaultId: string; 
    teamVaultName: string; 
    teamVaultDescription: string; 
    currentStatus: string;
}

interface UpdateTeamVaultResponse { 
    teamVaultName: string; 
    teamVaultDescription: string; 
    currentStatus: string; 
    success: boolean;
}

interface GetTeamVaultRequest { 
    teamId: string; 
}

interface GetTeamVaultResponse { 
    teamVaultId: string;
    teamVaultName: string; 
    teamVaultDescription: string; 
    currentStatus: string; 
    success: boolean;
}

function BuildCreateTeamRequest(teamId: string, teamVaultDescription: string, teamVaultName: string, currentStatus: string): CreateTeamVaultRequest { 

    const createTeam: CreateTeamVaultRequest = {
        teamId: teamId,
        teamVaultDescription: teamVaultDescription,
        teamVaultName: teamVaultName,
        currentStatus: currentStatus
    }; 

    return createTeam;

}

function BuildCreateTeamResponse(teamId: string, teamVaultId: string, teamVaultDescription: string, teamVaultName: string, currentStatus: string, success: boolean): CreateTeamVaultResponse { 

    const teamResponse: CreateTeamVaultResponse = {
        teamId: teamId,
        teamVaultId: teamVaultId,
        teamVaultDescription: teamVaultDescription,
        teamVaultName: teamVaultName,
        currentStatus: currentStatus,
        success: success
    }; 

    return teamResponse;

}

export async function CreateTeamVault(teamId: string, teamVaultDescription: string, teamVaultName: string, currentStatus: string): Promise<CreateTeamVaultResponse> { 

    const requestBody = BuildCreateTeamRequest(teamId, teamVaultDescription, teamVaultName, currentStatus);

    const createTeamVault = await fetch("/TeamVault/CreateTeamVault", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(requestBody)
    }); 

    if (!createTeamVault.ok) { 

        const errorText = await createTeamVault.text();

        throw new Error(errorText);

    }

    const jsonBody = await createTeamVault.json();

    const response = BuildCreateTeamResponse(jsonBody.teamId, jsonBody.teamVaultId, jsonBody.teamVaultDescription, jsonBody.teamVaultName, jsonBody.currentStatus, jsonBody.success); 

    return response;
}

function BuildGetTeamsWithNoVaultsResponse(teamsWithNoVaults : Team[], success: boolean) { 

    const response: GetTeamsWithNoVaultsResponse = {
        teamsWithNoVaults: teamsWithNoVaults,
        success: success
    }; 

    return response;

}

export async function GetTeamsWithNoVaults() : Promise<GetTeamsWithNoVaultsResponse> { 

    const getTeamsNoVaults = await fetch("/TeamVault/GetTeamsWithNoVaults", {
        method: "GET",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include"
    }); 

    if (!getTeamsNoVaults.ok) { 

        const errorText = await getTeamsNoVaults.text();

        throw new Error(errorText);
    }

    const jsonBody = await getTeamsNoVaults.json();

    const response = BuildGetTeamsWithNoVaultsResponse(jsonBody.teamsWithNoVaults, jsonBody.success); 

    return response;



}

function BuildDeleteTeamVaultRequest(teamVaultId: string): DeleteTeamVaultRequest { 

    const request: DeleteTeamVaultRequest = {
        teamVaultId: teamVaultId
    }; 

    return request;

}

function BuildDeleteTeamVaultResponse(teamVaultId: string, success: boolean): DeleteTeamVaultResponse { 

    const response: DeleteTeamVaultResponse = {
        teamVaultId: teamVaultId,
        success: success
    }; 

    return response;

}

export async function DeleteTeamVault(teamVaultId: string): Promise<DeleteTeamVaultResponse> { 

    const buildRequest = BuildDeleteTeamVaultRequest(teamVaultId);

    const deleteTeamVault = await fetch("/TeamVault/DeleteTeamVault", {
        method: "DELETE",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildRequest)
    }); 

    if (!deleteTeamVault.ok) { 

        const errorText = await deleteTeamVault.text();

        throw new Error(errorText);

    }

    const jsonBody = await deleteTeamVault.json();

    const response = BuildDeleteTeamVaultResponse(jsonBody.teamVaultId, jsonBody.success); 

    return response;

}

function BuildUpdateTeamVaultRequest(teamVaultId: string, teamVaultName: string, teamVaultDescription: string, currentStatus: string): UpdateTeamVaultRequest { 

    const teamVault: UpdateTeamVaultRequest = {
        teamVaultId: teamVaultId,
        teamVaultName: teamVaultName,
        teamVaultDescription: teamVaultDescription,
        currentStatus: currentStatus
    }; 

    return teamVault;

}

function BuildUpdateTeamVaultResponse(teamVaultName: string, teamVaultDescription: string, currentStatus: string, success: boolean): UpdateTeamVaultResponse { 

    const response: UpdateTeamVaultResponse = {
        teamVaultName: teamVaultName,
        teamVaultDescription: teamVaultDescription,
        currentStatus: currentStatus,
        success: success
    }; 

    return response;

}

export async function UpdateTeamVault(teamVaultId: string, teamVaultName: string, teamVaultDescription: string, currentStatus: string): Promise<UpdateTeamVaultResponse> { 

    const requestBody = BuildUpdateTeamVaultRequest(teamVaultId, teamVaultName, teamVaultDescription, currentStatus);

    const updateRequest = await fetch("/TeamVault/UpdateTeamVault", {
        method: "PUT",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(requestBody)

    }); 

    if (!updateRequest.ok) { 


        const errorText = await updateRequest.text();

        throw new Error(errorText);
    }

    const jsonBody = await updateRequest.json();

    const buildResponse = BuildUpdateTeamVaultResponse(jsonBody.teamVaultName, jsonBody.teamVaultDescription, jsonBody.currentStatus, jsonBody.success); 

    return buildResponse;

}

function BuildGetTeamVaultRequest(teamId: string): GetTeamVaultRequest { 

    const newRequest: GetTeamVaultRequest = {
        teamId: teamId
    }; 

    return newRequest;

}

function BuildGetTeamVaultResponse(teamVaultId: string, teamVaultName: string,teamVaultDescription: string,currentStatus: string, success: boolean): GetTeamVaultResponse { 
    const newResponse: GetTeamVaultResponse = {
        teamVaultId: teamVaultId,
        teamVaultName: teamVaultName,
        teamVaultDescription: teamVaultDescription,
        currentStatus: currentStatus,
        success: success
    }; 

    return newResponse;
}

export async function GetTeamVault(teamId: string): Promise<GetTeamVaultResponse> { 

    const requestBody = BuildGetTeamVaultRequest(teamId);

    const fetchTeamVault = await fetch("/TeamVault/GetTeamVault", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(requestBody)
    }); 

    if (!fetchTeamVault.ok) { 
        const errorText = await fetchTeamVault.text();

        throw new Error(errorText);
    }

    const jsonBody = await fetchTeamVault.json();

    const response = BuildGetTeamVaultResponse(jsonBody.teamVaultId, jsonBody.teamVaultName, jsonBody.teamVaultDescription, jsonBody.currentStatus, jsonBody.success);

    return response;

}