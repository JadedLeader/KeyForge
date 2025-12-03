interface CreateTeamInviteRequest { 
    teamVaultId: string; 
    inviteRecipient: string;
}

interface CreateTeamInviteResponse { 
    inviteRecipient: string; 
    inviteCreatedAt: string; 
    success: boolean;
}

interface GetPendingTeamInvitesRequest { 
    teamVaultId: string;
}


type TeamInvites = { 
    inviteSentBy: string; 
    inviteRecipient: string; 
    inviteCreatedAt: string;
    teamInviteId: string;
}
interface GetPendingTeamInvitesResponse { 
    pendingTeamInvites: TeamInvites[]; 
    success: boolean;
}

interface GetAllPendingInvitesForAccountRequest { 
    email: string;
}

interface GetAllPendingInvitesForAccountResponse { 
    pendingTeamInvites: TeamInvites[]; 
    success: boolean;
}

interface UpdateTeamInviteRequest { 
    teamInviteId: string; 
    inviteStatus: string;
}

interface UpdateTeamInviteResponse { 
    inviteStatus: string; 
    success: boolean;
}

function BuildCreateTeamInviteRequest(teamVaultId: string, inviteRecipient: string): CreateTeamInviteRequest { 

    const teamInvite: CreateTeamInviteRequest = {
        teamVaultId: teamVaultId,
        inviteRecipient: inviteRecipient
    }; 

    return teamInvite;

}

function BuildCreateTeamInviteResponse(inviteRecipient: string, inviteCreatedAt: string, success: boolean) : CreateTeamInviteResponse{ 

    const teamInviteResponse: CreateTeamInviteResponse = {
        inviteRecipient: inviteRecipient,
        inviteCreatedAt: inviteCreatedAt,
        success: success
    }; 

    return teamInviteResponse;

}

export async function CreateTeamInvite(teamVaultId: string, inviteRecipient: string): Promise<CreateTeamInviteResponse> { 

    const createTeamInvite = BuildCreateTeamInviteRequest(teamVaultId, inviteRecipient);

    const createTeamRequest = await fetch("/TeamInvite/CreateTeamInvite", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(createTeamInvite)
    }); 

    if (!createTeamRequest.ok) { 

        const errorText = await createTeamRequest.text(); 

        throw new Error(errorText);

    }

    const jsonBody = await createTeamRequest.json();

    const response = BuildCreateTeamInviteResponse(jsonBody.inviteRecipient, jsonBody.inviteCreatedAt, jsonBody.success);

    return response;

}

function BuildGetPendingTeamInvitesRequest(teamVaultId: string): GetPendingTeamInvitesRequest {

    const request: GetPendingTeamInvitesRequest = {
        teamVaultId: teamVaultId
    }; 

    return request;

}

function BuildGetPendingTeamInvitesResponse(pendingTeamInvites: TeamInvites[], success: boolean): GetPendingTeamInvitesResponse { 

    const response: GetPendingTeamInvitesResponse = {
        pendingTeamInvites: pendingTeamInvites,
        success: success
    }; 

    return response;

}

export async function GetPendingTeamInvites(teamVaultId: string): Promise<GetPendingTeamInvitesResponse> { 

    const buildingRequest = BuildGetPendingTeamInvitesRequest(teamVaultId);

    const getPendingInvites = await fetch("/TeamInvite/GetPendingTeamInvites", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildingRequest)
    }); 

    if (!getPendingInvites.ok) { 
        const errorText = await getPendingInvites.text();

        throw new Error(errorText);
    }

    const jsonBody = await getPendingInvites.json();

    const response = BuildGetPendingTeamInvitesResponse(jsonBody.pendingTeamInvites, jsonBody.success); 

    return response;

}

function BuildGetAllPendingInvitesForAccountRequest(email: string): GetAllPendingInvitesForAccountRequest { 

    const request: GetAllPendingInvitesForAccountRequest = {
        email: email
    };

    return request;

}

function BuildGetAllPendingInvitesForAccountResponse(pendingTeamInvites: TeamInvites[], success: boolean): GetAllPendingInvitesForAccountResponse { 
    const response: GetAllPendingInvitesForAccountResponse = {
        pendingTeamInvites: pendingTeamInvites,
        success: success
    }; 

    return response;
}

export async function GetPendingInvitesForAccount(email: string) { 

    const requestBody = BuildGetAllPendingInvitesForAccountRequest(email); 

    const request = await fetch("/TeamInvite/GetAllPendingInvitesForAccount", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(requestBody)
    }); 

    if (!request.ok) { 

        const errorText = await request.text(); 

        throw new Error(errorText);

    }

    const jsonBody = await request.json(); 

    const response = BuildGetAllPendingInvitesForAccountResponse(jsonBody.pendingTeamInvites, jsonBody.success);

    return response;


}

function BuildUpdateTeamInviteRequest(teamInviteId: string, inviteStatus: string): UpdateTeamInviteRequest { 
    const request: UpdateTeamInviteRequest = {
        teamInviteId: teamInviteId,
        inviteStatus: inviteStatus
    }; 

    return request;
}

function BuildUpdateTeamInviteResponse(inviteStatus: string, success: boolean): UpdateTeamInviteResponse { 
    const response: UpdateTeamInviteResponse = {
        inviteStatus: inviteStatus,
        success: success
    }; 

    return response;
}

export async function UpdateTeamInvite(teamInviteId: string, inviteStatus: string): Promise<UpdateTeamInviteResponse> { 

    const requestBody = BuildUpdateTeamInviteRequest(teamInviteId, inviteStatus); 

    const request = await fetch("/TeamInvite/UpdateTeamInvite", {
        method: "PUT",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(requestBody)

    });

    if (!request.ok) { 
        const errorText = await request.text(); 

        throw new Error(errorText);
    }

    const jsonBody = await request.json(); 

    const response = BuildUpdateTeamInviteResponse(jsonBody.inviteStatus, jsonBody.success);

    return response;
}
