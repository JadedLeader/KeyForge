
interface SilentTokenCycleRequest {

}

interface SilentTokenCycleResponse {
    refreshedToken: string;
    successful: boolean;
}

function BuildSilentCycleRequest(): SilentTokenCycleRequest {


    const newCycleRequest: SilentTokenCycleRequest = {

    };

    return newCycleRequest;
}


export async function SilentTokenRefresh(): Promise<SilentTokenCycleResponse> {


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

    }

    return await silentTokenResponse.json();


}