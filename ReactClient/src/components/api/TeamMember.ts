interface CreateTeamMemberRequest { 
    teamVaultId: string | null; 
    teamInviteId: string;
}

interface CreateTeamMemberResponse { 
    username: string; 
    email: string; 
    success: boolean;
}

function BuildCreateTeamMemberRequest(teamVaultId: string, teamInviteId: string): CreateTeamMemberRequest { 
    const request: CreateTeamMemberRequest = {
        teamVaultId: teamVaultId,
        teamInviteId: teamInviteId
    }; 

    return request;
}

function BuildCreateTeamMemberResponse(username: string, email: string, success: boolean): CreateTeamMemberResponse { 
    const response: CreateTeamMemberResponse = {
        username: username,
        email: email,
        success: success
    }; 

    return response;
}

export async function CreateTeamMember(teamVaultId: string, teamInviteId: string): Promise<CreateTeamMemberResponse> { 

    const buildRequest = BuildCreateTeamMemberRequest(teamVaultId, teamInviteId); 

    const request = await fetch("TeamMember/CreateTeamMember", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildRequest)
    }); 

    if (!request.ok) { 
        const errorText = await request.text(); 

        throw new Error(errorText);
    }

    const jsonBody = await request.json(); 

    const response = BuildCreateTeamMemberResponse(jsonBody.username, jsonBody.email, jsonBody.success); 

    return response;

}