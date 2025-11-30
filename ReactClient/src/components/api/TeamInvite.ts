interface CreateTeamInviteRequest { 
    teamVaultId: string; 
    inviteRecipient: string;
}

interface CreateTeamInviteResponse { 
    inviteRecipient: string; 
    inviteCreatedAt: string; 
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

