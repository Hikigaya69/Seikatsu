import { Outlet } from "react-router-dom";
import "@fontsource/poppins";

import AdminSidebar from "../components/admin/AdminSidebar";
import AdminNavbar from "../components/admin/AdminNavbar";

export default function AdminLayout() {

  return (

    <div
  className="flex min-h-screen bg-[#f5f5f7] text-[#111827]"
  style={{
    fontFamily:"Poppins"
  }}
>

      <AdminSidebar />

      <div className="flex-1 flex flex-col">

        <AdminNavbar />

        <main className="p-8">
          <Outlet />
        </main>

      </div>

    </div>

  );

}