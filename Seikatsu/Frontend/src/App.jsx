import { BrowserRouter, Routes, Route } from "react-router-dom";
import Landing from "./pages/Landing";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Home from "./pages/Home";
import ProductDetails from "./pages/ProductDetails";
import Cart from "./pages/Cart";
import Checkout from "./pages/Checkout";
import Profile from "./pages/Profile";
import OrderSummary from "./pages/OrderSummary";
import ForgotPassword from "./pages/ForgotPassword";
import ResetPassword from "./pages/ResetPassword";
import Checklist from "./pages/Checklist";
import RestockCart from "./pages/RestockCart";
import AdminLayout from "./layouts/AdminLayout";
import Dashboard from "./pages/admin/Dashboard";
import Products from "./pages/admin/Products";
import Categories from "./pages/admin/Categories";
import Analytics from "./pages/admin/Analytics";
import Inventory from "./pages/admin/Inventory";
import RestockManagement from "./pages/admin/RestockManagement";
import PaymentSuccess from "./pages/PaymentSuccess";


function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Landing />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                 <Route path="/home" element={<Home />} />
                 <Route path="/product/:id" element={<ProductDetails />} />
                 <Route path="/cart" element={<Cart />} />
                <Route path="/checkout" element={<Checkout />} />
                <Route path="/profile" element={<Profile />} />
                <Route path="/payment-success/:orderId" element={<PaymentSuccess />} />
                <Route path="/order-summary/:orderId" element={<OrderSummary />} />
                <Route path="/forgot-password" element={<ForgotPassword />} />
                <Route path="/reset-password" element={<ResetPassword />} />
                <Route path="/checklist" element={<Checklist />} />
                <Route path="/restock-cart" element={<RestockCart />} />
                <Route path="/admin" element={<AdminLayout />}>
                

  <Route index element={<Dashboard />} />
                    
  <Route path="products" element={<Products />} />

  <Route path="categories" element={<Categories />} />

  <Route path="inventory" element={<Inventory />} />

  <Route
    path="restock-management"
    element={<RestockManagement />}
  />

  <Route path="analytics" element={<Analytics />} />

</Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;