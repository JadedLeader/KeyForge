
interface SilentTokenCycleRequest {

}

interface SilentTokenCycleResponse {
    refreshedToken: string;
    successful: boolean;
}

interface RefreshShortLivedTokenRequest {
    accountId: string;
}

interface RefreshShortLivedTokenResponse {

    accountId: string;
    successful: boolean;
    refreshedToken: string;
}

interface BuildTokenGenerationRequest {

    accountId: string;

}

interface BuildTokenGenerationResponse {
    accountId: string;
    shortLivedToken: string;
    longLivedToken: string;
    successful: boolean;
    details: string;

}

interface UserLoginRequest {

    username: string;
    password: string;
}


interface UserLoginResponse {
    accountId: string;
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

export async function FetchShortLivedTokenRefresh(shortLivedTokenRequest: RefreshShortLivedTokenRequest): Promise<RefreshShortLivedTokenResponse> {


    const fetchShortLivedTokenEndpoint = await fetch("/Auth/UpdateShortLivedKey", {
        method: "PUT",
        headers: {
            'Content-type': "application/json"
        },
        body: JSON.stringify(shortLivedTokenRequest),
    });

    if (!fetchShortLivedTokenEndpoint.ok) {

        const errorText = await fetchShortLivedTokenEndpoint.text();

        throw new Error(errorText);

    }

    const responseData = await fetchShortLivedTokenEndpoint.json() as RefreshShortLivedTokenResponse;

    return responseData;

}

export async function FetchAuthEndpoint(buildTokenGenerationRequest: BuildTokenGenerationRequest): Promise<BuildTokenGenerationResponse> {

    const fetchAuth = await fetch("/Auth/CreateInitialKey", {
        method: "POST",
        headers: {
            'Content-type': "application/json"
        },
        body: JSON.stringify(buildTokenGenerationRequest),
    });

    if (!fetchAuth.ok) {

        const authErrorText = await fetchAuth.text();

        throw new Error(authErrorText);

    }

    const authResponseData = await fetchAuth.json() as BuildTokenGenerationResponse;


    return authResponseData;

}

export async function FetchLoginEndpoint(loginRequest: UserLoginRequest): Promise<UserLoginResponse> {

    const fetchLogin = await fetch("/Auth/UserLogin", {
        method: "POST",
        headers: {
            'Content-type': "application/json"
        },
        body: JSON.stringify(loginRequest),
    });

    if (!fetchLogin.ok) {

        const errorText = await fetchLogin.text();

        throw new Error(errorText);
    }

    const responseData = await fetchLogin.json() as UserLoginResponse;

    console.log("retrieved account login response");
    console.log("Account ID", responseData.accountId);

    return responseData;


}