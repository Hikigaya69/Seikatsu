import axios from 'axios';

const PUBLIC_ROUTES = [
    '/api/Auth/login',
    '/api/Auth/register',
    '/api/Auth/refresh-token',
    '/api/Auth/forgot-password',
    '/api/Auth/reset-password',
    'api/Auth/check'
];

const isPublicRoute = (url) => {
    return PUBLIC_ROUTES.some(route => url.includes(route));
};

const api = axios.create({
    baseURL: '',
    withCredentials: true,
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error) => {
    failedQueue.forEach(({ resolve, reject }) => {
        error ? reject(error) : resolve();
    });
    failedQueue = [];
};

api.interceptors.response.use(
    (response) => response,

    async (error) => {
        const originalRequest = error.config;

        if (
            error.response?.status === 401 &&
            !originalRequest._retry &&
            !isPublicRoute(originalRequest.url) // ← key check
        ) {
            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                }).then(() => api(originalRequest));
            }

            originalRequest._retry = true;
            isRefreshing = true;

            try {
                await api.post('/api/Auth/refresh-token');
                processQueue(null);
                return api(originalRequest);
            } catch (refreshError) {
                processQueue(refreshError);
                window.location.href = '/login';
                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
            }
        }

        return Promise.reject(error);
    }
);

export default api;