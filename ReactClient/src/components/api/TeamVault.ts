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