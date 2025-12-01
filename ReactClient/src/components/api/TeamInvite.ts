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

