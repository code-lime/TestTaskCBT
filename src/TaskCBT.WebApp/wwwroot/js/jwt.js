/**
 * @param {string} token
 * @returns {(object | undefined)}
 */
function getParsedJwt(token) {
    try {
        return JSON.parse(atob(token.split('.')[1]))
    } catch (error) {
        return undefined
    }
}
/**
 * @returns {(object | undefined)}
 */
function getParsedCurrentJwt() {
    let accessToken = localStorage.getItem('accessToken');
    return accessToken !== null
        ? getParsedJwt(accessToken)
        : undefined;
}
/**
 * @param {({accessToken:string,refreshToken:string}|null)} auth
 */
function setCurrentJwt(auth) {
    if (auth === null) {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        return;
    }
    accessToken = auth.accessToken;
    refreshToken = auth.refreshToken;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
}
/**
 * @param {string} refreshApi
 * @param {function()} unauthCallback
 */
function initFetchJwt(refreshApi, unauthCallback) {
    const originalFetch = fetch;
    
    fetch = function () {
        let self = this;
        let args = arguments;
        let init = args.length >= 2 ? args[1] : null;
        if (init !== null && init !== undefined && 'jwt' in init) {
            let jwt = init['jwt'];
            let accessToken;
            let refreshToken;
            if (jwt === 'local') {
                accessToken = localStorage.getItem('accessToken');
                refreshToken = localStorage.getItem('refreshToken');
            } else {
                accessToken = jwt.accessToken;
                refreshToken = jwt.refreshToken;
            }
            let headers;
            if ('headers' in init) headers = init.headers;
            else init.headers = headers = {};

            headers['Authorization'] = 'Bearer ' + accessToken;

            return originalFetch.apply(self, args).then(async function (data) {
                if (data.status !== 401) return data;
                if (refreshToken === undefined) return data;
                let response = await originalFetch(`${refreshApi}?refreshToken=${encodeURIComponent(refreshToken)}`);
                if (response.status === 401) {
                    unauthCallback();
                    return response;
                }
                if (response.status !== 200) return response;
                let json = await response.json();
                if (json.status !== 'success') return response;
                let auth = json.response;
                accessToken = auth.accessToken;
                if (jwt === 'local') setCurrentJwt(auth);
                headers['Authorization'] = 'Bearer ' + accessToken;
                response = await originalFetch(...args);
                return response
            });
        }
        return originalFetch(...args);
    };
}