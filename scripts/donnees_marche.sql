--
-- PostgreSQL database dump
--

\restrict Pomq8421IapecDOrk4SiL9eMAwSfXqd9XyfdSAHFnMQXeEXCleW17he3d5UCVW8

-- Dumped from database version 17.11
-- Dumped by pg_dump version 17.11

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: emetteur; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.emetteur (id, nom, sigle, type, pays_code, site_web) FROM stdin;
01a1fecb-6a4a-7cea-b700-dcee97b6c685	État du Cameroun	ECMR	Etat	CM	\N
01a1fecb-6aff-7519-9a85-088ea5329399	État du Gabon	EOG	Etat	GA	\N
01a1fecb-6b0c-78f8-9f1b-496b4f729d6b	État du Tchad	EOTD	Etat	TD	\N
01a1fecb-6b10-7599-8fe3-983799ea695b	Banque de Développement des États de l'Afrique Centrale	BDEAC	Supranational	CG	\N
01a1fecb-6b14-7a32-b990-b82b5f00d7c0	Société Camerounaise de Crédit Automobile	ALIOS FINANCE	Entreprise	CM	\N
01a1fecb-6b18-73d6-a156-10c23943dcc0	Agence de Crédit pour l'Entreprise Privée Cameroun	ACEP CAMEROUN	Entreprise	CM	\N
01a1fecb-6b1d-78f5-98d9-fd22f153205b	Société Nationale des Pétroles du Congo	SNPC	Entreprise	CG	\N
01a1fecb-6b21-78bd-bce2-2a90df2d807f	Société des Eaux Minérales du Cameroun	SEMC	Entreprise	CM	\N
01a1fecb-6b25-7fa6-aeee-576fcfe0cbfb	Société Africaine Forestière et Agricole du Cameroun	SAFACAM	Entreprise	CM	\N
01a1fecb-6b29-7316-8db3-863cc7206c83	Société Camerounaise de Palmeraies	SOCAPALM	Entreprise	CM	\N
01a1fecb-6b2e-77d2-929e-b24dc7b923bb	La Régionale Bank	LA REGIONALE	Entreprise	CM	\N
01a1fecb-6b32-76c5-8eda-6914c82114f2	Banco Nacional de Guinea Ecuatorial	BANGE	Entreprise	GQ	\N
01a1fecb-6b36-729e-b631-4cd9aa4e1daa	Société Commerciale Gabonaise de Réassurance	SCG-Ré	Entreprise	GA	\N
01a1fecb-6b3b-7ba7-a78c-a61e4b72237f	BGFI Holding Corporation	BGFI HC	Entreprise	GA	\N
\.


--
-- Data for Name: titre; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.titre (id, isin, nom, emetteur_id, type_titre, nominal, date_admission, statut) FROM stdin;
01a1fecc-25e1-7355-8fc9-99eac58e8f0f	CM0000020305	ECMR 6,25% NET 2022-2029	01a1fecb-6a4a-7cea-b700-dcee97b6c685	Obligation	10000.00	2022-09-28	Actif
01a1fecc-26c0-7455-a5ea-efd5b6f865b6	GA0000020313	EOG 6,25% NET 2022-2028	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2023-01-26	Actif
01a1fecc-26cd-7031-a355-04de49476521	TD0000020331	EOTD 6,5% NET 2022-2027	01a1fecb-6b0c-78f8-9f1b-496b4f729d6b	Obligation	10000.00	2023-03-24	Actif
01a1fecc-26d5-77ee-a75d-2d9e2d084b8f	CM0000020362	ECMR 6,00% NET 2023-2027	01a1fecb-6a4a-7cea-b700-dcee97b6c685	Obligation	10000.00	2023-11-23	Actif
01a1fecc-26de-7a24-9b6b-21492e00a4f1	CM0000020370	ECMR 6,75% NET 2023-2029	01a1fecb-6a4a-7cea-b700-dcee97b6c685	Obligation	10000.00	2023-11-23	Actif
01a1fecc-26e6-7c8f-9c93-375def92f606	CM0000020388	ECMR 7,25% NET 2023-2031	01a1fecb-6a4a-7cea-b700-dcee97b6c685	Obligation	10000.00	2023-11-23	Actif
01a1fecc-26ef-7dd3-b13a-3ae72ac00dd5	GA0000020396	EOG 6,25% NET 2023-2028	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2024-12-30	Actif
01a1fecc-26f7-72d2-a8ae-50ce8ca83ae5	GA0000020487	EOG 6% NET 2024-2027	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2024-12-30	Actif
01a1fecc-2701-7f99-bce8-56eecf258461	GA0000020495	EOG 6,5% NET 2024-2029	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2024-12-30	Actif
01a1fecc-270a-7ca4-b463-c5aa658dc053	GA0000020503	EOG 7,5% NET 2024-2031	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2024-12-30	Actif
01a1fecc-2716-7151-ad0d-1f3dcf8fe050	GA0000020511	EOG MT 6,60% NET 2024-2027	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2024-12-30	Actif
01a1fecc-271f-7c15-8d4b-a8226f563658	GA0000020529	EOG MT 6,75% NET 2024-2028	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2024-12-30	Actif
01a1fecc-2729-712f-9bfa-6e81ed984c1e	GA0000020537	EOG MT 7% NET 2024-2030	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2024-12-30	Actif
01a1fecc-2731-7bce-9935-c9474aa06fff	GA0000020552	EOG MT 6,6% NET 2024-2027-II	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2025-05-30	Actif
01a1fecc-2743-77b4-b6c7-2aad48fcc6f5	GA0000020560	EOG MT 6,75% NET 2024-2028-II	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2025-05-30	Actif
01a1fecc-274d-7f61-9cb1-67cb4451012a	GA0000020578	EOG MT 7,00% NET 2024-2030-II	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2025-05-30	Actif
01a1fecc-2758-7f66-a7bd-10ab89eaf085	GA0000020594	EOG 5,6% NET 2025-2027	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2026-02-23	Actif
01a1fecc-2763-791f-9026-3e21c0392d1f	GA0000020602	EOG 6% NET 2025-2028	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2026-02-23	Actif
01a1fecc-276e-7043-8363-6b69eedbc4d6	GA0000020636	EOG 5,6% NET 2025-2027 II	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2026-05-18	Actif
01a1fecc-2779-7fa0-b662-453e4ad7befc	GA0000020644	EOG 6% NET 2025-2028 II	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2026-05-18	Actif
01a1fecc-2785-7b08-aa87-8eb293491425	GA0000020651	EOG 6,00% NET 2026-2029	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2026-11-03	Actif
01a1fecc-2791-7a1f-a309-fc73fed4b904	GA0000020669	EOG 6,50% NET 2026-2030	01a1fecb-6aff-7519-9a85-088ea5329399	Obligation	10000.00	2026-11-03	Actif
01a1fecc-279c-74ca-a22d-6996527918b7	CG0000020220	BDEAC 5,45% NET 2020-2027	01a1fecb-6b10-7599-8fe3-983799ea695b	Obligation	10000.00	2021-02-03	Actif
01a1fecc-27a6-7e1a-8118-cc979f3c1f50	CG0000020261	BDEAC 5,6% NET 2021-2028	01a1fecb-6b10-7599-8fe3-983799ea695b	Obligation	10000.00	2022-03-11	Actif
01a1fecc-27ad-7d16-a463-7a1e9e79838e	CG0000020329	BDEAC 6% NET 2022-2029	01a1fecb-6b10-7599-8fe3-983799ea695b	Obligation	10000.00	2023-02-15	Actif
01a1fecc-27b4-721b-a30d-40b3fde24ab5	CG0000020469	BDEAC 6,20% NET 2024-2031	01a1fecb-6b10-7599-8fe3-983799ea695b	Obligation	10000.00	2024-08-07	Actif
01a1fecc-27bb-7f23-a017-ae9c0f1c9cf2	CG0000020436	BDEAC 5,95% NET 2024-2029	01a1fecb-6b10-7599-8fe3-983799ea695b	Obligation	10000.00	2024-08-07	Actif
01a1fecc-27c1-7c7d-8944-093e2d3fec72	CG0000020444	BDEAC 4,70% NET 2024-2027	01a1fecb-6b10-7599-8fe3-983799ea695b	Obligation	10000.00	2024-08-07	Actif
01a1fecc-27c8-7edd-b6ae-a5dea8ac6437	CM0000020412	ALIOS 6,5% BRUT 2023-2028	01a1fecb-6b14-7a32-b990-b82b5f00d7c0	Obligation	10000.00	2024-02-05	Actif
01a1fecc-27cd-7e87-8dcd-621937963201	CM0000020545	ACEP 7% BRUT 2024-2027	01a1fecb-6b18-73d6-a156-10c23943dcc0	Obligation	10000.00	2025-04-08	Actif
01a1fecc-27d2-7876-9e39-f53b1b047477	CG0000020584	SNPC 6,5% NET 2024-2029	01a1fecb-6b1d-78f5-98d9-fd22f153205b	Obligation	10000.00	2025-10-15	Actif
01a1fecc-27d9-7943-8f06-4470e1e4502b	CM0000020610	ALIOS-05 6% BRUT 2025-2028	01a1fecb-6b14-7a32-b990-b82b5f00d7c0	Obligation	10000.00	2025-12-19	Actif
01a1fecc-27de-7bc6-a2d6-ae9ad92aa294	CM0000020628	ALIOS-06 7% BRUT 2025-2030	01a1fecb-6b14-7a32-b990-b82b5f00d7c0	Obligation	10000.00	2025-12-19	Actif
01a1fecc-b8cd-72c4-b69e-621d04400213	CM0000010009	SEMC	01a1fecb-6b21-78bd-bce2-2a90df2d807f	Action	10000.00	2006-06-30	Actif
01a1fecc-b8eb-7fc8-af40-557c9947af87	CM0000010017	SAFACAM	01a1fecb-6b25-7fa6-aeee-576fcfe0cbfb	Action	5000.00	2008-07-09	Actif
01a1fecc-b8f2-7ebd-949b-53e5f05d8694	CM0000010025	SOCAPALM	01a1fecb-6b29-7316-8db3-863cc7206c83	Action	10000.00	2009-04-07	Actif
01a1fecc-b8f9-75d6-b1f1-32035b297839	CM0000010041	LA REGIONALE	01a1fecb-6b2e-77d2-929e-b24dc7b923bb	Action	10000.00	2021-07-16	Actif
01a1fecc-b8ff-7cd5-b178-6e33a03fe1d1	GQ0000010050	BANGE	01a1fecb-6b32-76c5-8eda-6914c82114f2	Action	100000.00	2022-09-28	Actif
01a1fecc-b905-7f3a-bab1-02db4f4dc132	GA0000010066	SCG-Ré	01a1fecb-6b36-729e-b631-4cd9aa4e1daa	Action	10000.00	2023-01-26	Actif
01a1fecc-b90b-76a3-8180-93b68d2683a1	GA0000010074	BGFI HC	01a1fecb-6b3b-7ba7-a78c-a61e4b72237f	Action	10000.00	2026-05-07	Actif
\.


--
-- Data for Name: action; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.action (titre_id, nombre_titres) FROM stdin;
01a1fecc-b8cd-72c4-b69e-621d04400213	192473
01a1fecc-b8eb-7fc8-af40-557c9947af87	1242000
01a1fecc-b8f2-7ebd-949b-53e5f05d8694	4575789
01a1fecc-b8f9-75d6-b1f1-32035b297839	1012536
01a1fecc-b8ff-7cd5-b178-6e33a03fe1d1	558960
01a1fecc-b905-7f3a-bab1-02db4f4dc132	1500000
01a1fecc-b90b-76a3-8180-93b68d2683a1	14728385
\.


--
-- Data for Name: cours; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.cours (id, titre_id, date_seance, cours_reference, cours_cloture, volume, coupon_couru_publie) FROM stdin;
01a1fecc-fd36-75cf-b800-269f3733ac37	01a1fecc-25e1-7355-8fc9-99eac58e8f0f	2026-11-20	\N	100.0000	0	186.99
01a1fecc-fd6e-712e-ba72-0217557c9907	01a1fecc-26c0-7455-a5ea-efd5b6f865b6	2026-11-20	\N	100.0000	0	16.44
01a1fecc-fd74-762c-aa58-5e60d1d3eb0b	01a1fecc-26cd-7031-a355-04de49476521	2026-11-20	\N	100.0000	0	293.84
01a1fecc-fd78-7502-b0e2-0fc700628c8e	01a1fecc-26d5-77ee-a75d-2d9e2d084b8f	2026-11-20	\N	100.0000	0	127.40
01a1fecc-fd7b-7cb6-9752-70c716ae4e2f	01a1fecc-26de-7a24-9b6b-21492e00a4f1	2026-11-20	\N	100.0000	0	286.64
01a1fecc-fd7f-7d2a-a804-785ccad8b6d9	01a1fecc-26e6-7c8f-9c93-375def92f606	2026-11-20	\N	99.0000	0	307.88
01a1fecc-fd82-7df0-9da5-808439ce096e	01a1fecc-26ef-7dd3-b13a-3ae72ac00dd5	2026-11-20	\N	96.0000	0	113.87
01a1fecc-fd84-721f-8de2-9fdbacce6901	01a1fecc-26f7-72d2-a8ae-50ce8ca83ae5	2026-11-20	\N	100.0000	0	396.16
01a1fecc-fd87-77ee-8d21-e4f1a2e313d4	01a1fecc-2701-7f99-bce8-56eecf258461	2026-11-20	\N	100.0000	0	429.18
01a1fecc-fd8a-70d9-b417-c7dba184774c	01a1fecc-270a-7ca4-b463-c5aa658dc053	2026-11-20	\N	100.0000	0	495.21
01a1fecc-fd8c-7d2a-b36e-6116022d9fb7	01a1fecc-2716-7151-ad0d-1f3dcf8fe050	2026-11-20	\N	98.0000	0	132.90
01a1fecc-fd8f-7110-858f-c7ca41a328b0	01a1fecc-271f-7c15-8d4b-a8226f563658	2026-11-20	\N	100.0000	0	271.85
01a1fecc-fd92-714c-ae35-eecff9204c66	01a1fecc-2729-712f-9bfa-6e81ed984c1e	2026-11-20	\N	100.0000	0	281.92
01a1fecc-fd94-7865-b575-f3850f20b9aa	01a1fecc-2731-7bce-9935-c9474aa06fff	2026-11-20	\N	97.0000	0	596.71
01a1fecc-fd97-7514-9ea7-6e21b509455a	01a1fecc-2743-77b4-b6c7-2aad48fcc6f5	2026-11-20	\N	97.0000	0	610.27
01a1fecc-fd99-77b7-abc7-30bb90cbaed2	01a1fecc-274d-7f61-9cb1-67cb4451012a	2026-11-20	\N	100.0000	0	632.88
01a1fecc-fd9c-7694-94f9-c87e37afddf8	01a1fecc-2758-7f66-a7bd-10ab89eaf085	2026-11-20	\N	100.0000	0	359.01
01a1fecc-fd9e-7d06-94e4-1ce578df83d7	01a1fecc-2763-791f-9026-3e21c0392d1f	2026-11-20	\N	100.0000	0	384.66
01a1fecc-fda0-78b1-bf1d-4105da65e4be	01a1fecc-276e-7043-8363-6b69eedbc4d6	2026-11-20	\N	100.0000	0	84.38
01a1fecc-fda2-74ff-8ff1-d787e8f7b728	01a1fecc-2779-7fa0-b662-453e4ad7befc	2026-11-20	\N	100.0000	0	90.41
01a1fecc-fda4-707a-91e2-192681cb2c77	01a1fecc-2785-7b08-aa87-8eb293491425	2026-11-20	\N	100.0000	0	427.40
01a1fecc-fda6-7f54-bf03-dfbeaaa040aa	01a1fecc-2791-7a1f-a309-fc73fed4b904	2026-11-20	\N	100.0000	0	463.01
01a1fecc-fda8-7fe7-b8c2-306e7377a733	01a1fecc-279c-74ca-a22d-6996527918b7	2026-11-20	\N	100.0000	0	196.50
01a1fecc-fdab-77df-9fe2-2f11d87421a6	01a1fecc-27a6-7e1a-8118-cc979f3c1f50	2026-11-20	\N	98.5000	0	324.03
01a1fecc-fdad-7a28-b1eb-64a70ace518f	01a1fecc-27ad-7d16-a463-7a1e9e79838e	2026-11-20	\N	100.0000	0	433.97
01a1fecc-fdaf-7fbe-9e7c-296c0d18ee8b	01a1fecc-27b4-721b-a30d-40b3fde24ab5	2026-11-20	\N	100.0000	0	421.26
01a1fecc-fdb2-720f-99b0-c7ac247756d9	01a1fecc-27bb-7f23-a017-ae9c0f1c9cf2	2026-11-20	\N	100.6500	0	404.27
01a1fecc-fdb4-7e54-90b9-4b2c1ca09fbe	01a1fecc-27c1-7c7d-8944-093e2d3fec72	2026-11-20	\N	100.0000	0	319.34
01a1fecc-fdb6-7edf-9a4d-28d5573cd560	01a1fecc-27c8-7edd-b6ae-a5dea8ac6437	2026-11-20	\N	100.0000	0	89.93
01a1fecc-fdb8-7ade-b454-5c24cd1a933b	01a1fecc-27cd-7e87-8dcd-621937963201	2026-11-20	\N	100.0000	0	170.30
01a1fecc-fdba-7a13-829f-436eb0aeedf1	01a1fecc-27d2-7876-9e39-f53b1b047477	2026-11-20	\N	100.0000	0	468.71
01a1fecc-fdbc-74bd-b585-3a1266b7f049	01a1fecc-27d9-7943-8f06-4470e1e4502b	2026-11-20	\N	100.0000	0	20.71
01a1fecc-fdbe-77da-8ec9-9ed539c52003	01a1fecc-27de-7bc6-a2d6-ae9ad92aa294	2026-11-20	\N	100.0000	0	28.77
01a1fecc-fdc0-7ea1-a263-6fa26e328452	01a1fecc-b8cd-72c4-b69e-621d04400213	2026-11-20	\N	53000.0000	0	\N
01a1fecc-fdc3-7548-b07b-e0ed5c472768	01a1fecc-b8eb-7fc8-af40-557c9947af87	2026-11-20	\N	38500.0000	11	\N
01a1fecc-fdc4-79b6-97a4-05103ea64101	01a1fecc-b8f2-7ebd-949b-53e5f05d8694	2026-11-20	\N	49000.0000	60	\N
01a1fecc-fdc6-7cfb-ba6c-0b6e1268a141	01a1fecc-b8f9-75d6-b1f1-32035b297839	2026-11-20	\N	37499.0000	0	\N
01a1fecc-fdca-7a66-9328-f1aa7bb9394d	01a1fecc-b8ff-7cd5-b178-6e33a03fe1d1	2026-11-20	\N	228085.0000	0	\N
01a1fecc-fdcd-77af-a694-3c7aa3286cb7	01a1fecc-b905-7f3a-bab1-02db4f4dc132	2026-11-20	\N	20000.0000	2	\N
01a1fecc-fdcf-7cdc-81c7-f95aad7b3382	01a1fecc-b90b-76a3-8180-93b68d2683a1	2026-11-20	\N	87000.0000	5	\N
\.


--
-- Data for Name: dividende; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.dividende (id, titre_id, exercice, montant_brut, montant_net, date_detachement, date_paiement) FROM stdin;
01a1fecd-3247-70fb-b29c-43696b104653	01a1fecc-b8cd-72c4-b69e-621d04400213	2025	800.00	\N	\N	2026-06-25
01a1fecd-326b-77ae-8bd4-f5bd7fa6d41e	01a1fecc-b8eb-7fc8-af40-557c9947af87	2025	2200.00	\N	\N	2026-07-20
01a1fecd-3271-7a18-b025-bc508ece36df	01a1fecc-b8f2-7ebd-949b-53e5f05d8694	2025	2325.00	\N	\N	2026-07-20
01a1fecd-3274-7953-b512-cc9154a980c9	01a1fecc-b8f9-75d6-b1f1-32035b297839	2023	993.33	894.00	\N	2024-09-18
01a1fecd-3279-7160-8ad3-e5acb28fbcea	01a1fecc-b8ff-7cd5-b178-6e33a03fe1d1	2025	4830.00	\N	\N	2026-07-31
01a1fecd-327c-70d7-bb41-b5da370e39d2	01a1fecc-b905-7f3a-bab1-02db4f4dc132	2024	481.30	433.16	\N	2025-10-22
01a1fecd-327e-776b-b6b3-d83bf1a33211	01a1fecc-b90b-76a3-8180-93b68d2683a1	2025	2500.00	\N	\N	2026-05-18
\.


--
-- Data for Name: document_source; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.document_source (id, titre_id, type, libelle, url, date_publication) FROM stdin;
\.


--
-- Data for Name: jour_non_ouvre; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.jour_non_ouvre (date, libelle) FROM stdin;
\.


--
-- Data for Name: obligation; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.obligation (titre_id, taux_facial, periodicite, date_jouissance, date_premier_coupon, date_echeance, mode_remboursement, differe_echeances, code_convention, regime_fiscal) FROM stdin;
01a1fecc-25e1-7355-8fc9-99eac58e8f0f	6.250000	Annuelle	2022-05-27	\N	2029-05-27	AmortissementConstant	2	EXACT_365	Net
01a1fecc-26c0-7455-a5ea-efd5b6f865b6	6.250000	Annuelle	2022-11-01	\N	2028-11-01	AmortissementConstant	1	EXACT_365	Net
01a1fecc-26cd-7031-a355-04de49476521	6.500000	Annuelle	2022-12-30	\N	2027-12-30	AmortissementConstant	1	EXACT_365	Net
01a1fecc-26d5-77ee-a75d-2d9e2d084b8f	6.000000	Annuelle	2023-06-23	\N	2027-06-23	AmortissementConstant	2	EXACT_365	Net
01a1fecc-26de-7a24-9b6b-21492e00a4f1	6.750000	Annuelle	2023-06-23	\N	2029-06-23	AmortissementConstant	3	EXACT_365	Net
01a1fecc-26e6-7c8f-9c93-375def92f606	7.250000	Annuelle	2023-06-23	\N	2031-06-23	AmortissementConstant	3	EXACT_365	Net
01a1fecc-26ef-7dd3-b13a-3ae72ac00dd5	6.250000	Annuelle	2023-07-15	\N	2028-07-15	AmortissementConstant	1	EXACT_365	Net
01a1fecc-26f7-72d2-a8ae-50ce8ca83ae5	6.000000	Annuelle	2024-03-29	\N	2027-03-29	InFine	0	EXACT_365	Net
01a1fecc-2701-7f99-bce8-56eecf258461	6.500000	Annuelle	2024-03-29	\N	2029-03-29	AmortissementConstant	2	EXACT_365	Net
01a1fecc-270a-7ca4-b463-c5aa658dc053	7.500000	Annuelle	2024-03-29	\N	2031-03-29	AmortissementConstant	2	EXACT_365	Net
01a1fecc-2716-7151-ad0d-1f3dcf8fe050	6.600000	Annuelle	2024-07-01	\N	2027-07-01	AmortissementConstant	1	EXACT_365	Net
01a1fecc-271f-7c15-8d4b-a8226f563658	6.750000	Annuelle	2024-07-01	\N	2028-07-01	AmortissementConstant	2	EXACT_365	Net
01a1fecc-2729-712f-9bfa-6e81ed984c1e	7.000000	Annuelle	2024-07-01	\N	2030-07-01	AmortissementConstant	2	EXACT_365	Net
01a1fecc-2731-7bce-9935-c9474aa06fff	6.600000	Annuelle	2024-12-30	\N	2027-12-30	AmortissementConstant	1	EXACT_365	Net
01a1fecc-2743-77b4-b6c7-2aad48fcc6f5	6.750000	Annuelle	2024-12-30	\N	2028-12-30	AmortissementConstant	2	EXACT_365	Net
01a1fecc-274d-7f61-9cb1-67cb4451012a	7.000000	Annuelle	2024-12-30	\N	2030-12-30	AmortissementConstant	2	EXACT_365	Net
01a1fecc-2758-7f66-a7bd-10ab89eaf085	5.600000	Annuelle	2025-04-05	\N	2027-04-05	InFine	0	EXACT_365	Net
01a1fecc-2763-791f-9026-3e21c0392d1f	6.000000	Annuelle	2025-04-05	\N	2028-04-05	InFine	0	EXACT_365	Net
01a1fecc-276e-7043-8363-6b69eedbc4d6	5.600000	Annuelle	2025-10-01	\N	2027-10-01	InFine	0	EXACT_365	Net
01a1fecc-2779-7fa0-b662-453e4ad7befc	6.000000	Annuelle	2025-10-01	\N	2028-10-01	InFine	0	EXACT_365	Net
01a1fecc-2785-7b08-aa87-8eb293491425	6.000000	Annuelle	2026-03-10	\N	2029-03-10	InFine	0	EXACT_365	Net
01a1fecc-2791-7a1f-a309-fc73fed4b904	6.500000	Annuelle	2026-03-10	\N	2030-03-10	InFine	0	EXACT_365	Net
01a1fecc-279c-74ca-a22d-6996527918b7	5.450000	Annuelle	2020-12-31	\N	2027-12-31	AmortissementConstant	2	EXACT_365	Net
01a1fecc-27a6-7e1a-8118-cc979f3c1f50	5.600000	Annuelle	2021-12-08	\N	2028-12-08	AmortissementConstant	2	EXACT_365	Net
01a1fecc-27ad-7d16-a463-7a1e9e79838e	6.000000	Annuelle	2022-12-30	\N	2029-12-30	AmortissementConstant	2	EXACT_365	Net
01a1fecc-27b4-721b-a30d-40b3fde24ab5	6.200000	Annuelle	2024-03-22	\N	2031-03-22	AmortissementConstant	2	EXACT_365	Net
01a1fecc-27bb-7f23-a017-ae9c0f1c9cf2	5.950000	Annuelle	2024-03-22	\N	2029-03-22	AmortissementConstant	2	EXACT_365	Net
01a1fecc-27c1-7c7d-8944-093e2d3fec72	4.700000	Annuelle	2024-03-22	\N	2027-03-22	InFine	0	EXACT_365	Net
01a1fecc-27c8-7edd-b6ae-a5dea8ac6437	6.500000	Semestrielle	2023-08-16	\N	2028-08-16	AmortissementConstant	2	EXACT_365	Brut
01a1fecc-27cd-7e87-8dcd-621937963201	7.000000	Semestrielle	2024-12-30	\N	2027-12-30	AmortissementConstant	1	EXACT_365	Brut
01a1fecc-27d2-7876-9e39-f53b1b047477	6.500000	Annuelle	2024-12-31	\N	2029-12-31	AmortissementConstant	0	EXACT_365	Net
01a1fecc-27d9-7943-8f06-4470e1e4502b	6.000000	Trimestrielle	2025-11-07	\N	2028-08-07	AmortissementConstant	1	EXACT_365	Brut
01a1fecc-27de-7bc6-a2d6-ae9ad92aa294	7.000000	Trimestrielle	2025-11-07	\N	2030-08-07	AmortissementConstant	1	EXACT_365	Brut
\.


--
-- PostgreSQL database dump complete
--

\unrestrict Pomq8421IapecDOrk4SiL9eMAwSfXqd9XyfdSAHFnMQXeEXCleW17he3d5UCVW8

